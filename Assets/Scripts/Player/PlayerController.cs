using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerCollision))]
public class PlayerController : MonoBehaviour
{

    // Component references
    private Rigidbody2D _rb;
    private PlayerCollision _col;
    [SerializeField] private PlayerStats _stats;

    // Effects
    public GameObject LandEffect;
    public GameObject GroundJumpEffect;
    public GameObject AirJumpEffect;
    public GameObject DashEffect;

    #region INTERFACE

    public Vector2 Position => _rb.position;
    public Vector2 Velocity => _frameVelocity;
    public bool IsFacingRight => _isFacingRight;

    #endregion

    private float _time;
    private Vector2 _frameVelocity;
    private bool _isFacingRight = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<PlayerCollision>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        HandleCollisions();
        HandleHorizontal();
        HandleJump();
        HandleGravity();
        HandleDash();
        ApplyMovement();
    }

    private void ApplyMovement() => _rb.velocity = _frameVelocity;

    #region COLLISIONS

    // tracks if player was grounded last time step
    private bool _grounded;
    private float _timeLeftGround = float.MinValue;

    private void HandleCollisions()
    {
        if (_col.OnCeiling)
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        if (!_grounded && _col.OnGround)
            OnLanding();
        else if (_grounded && !_col.OnGround)
        {
            _timeLeftGround = _time;
        }

        if (_col.OnGround)
        {
            _jumpsRemaining = _stats.JumpCount;
            _dashAvailable = true;
        }

        _grounded = _col.OnGround;
    }

    /// <summary>
    /// This will run each time the player lands on the ground.
    /// </summary>
    private void OnLanding()
    {
        if (LandEffect != null) Instantiate(LandEffect, transform.position, Quaternion.identity);
        if (_frameVelocity.y <= -_stats.MaxFallSpeed)
            CameraShake.Instance.ShakeCamera(10f, 0.2f);
    }

    #endregion

    #region HORIZONTAL

    private void HandleHorizontal()
    {
        float moveSpeed = _stats.MoveSpeed;

        if (!_col.OnGround && Mathf.Abs(_frameVelocity.y) < _stats.JumpFloatWindow)
            moveSpeed *= _stats.JumpFloatMultiplier;

        _frameVelocity.x = Mathf.MoveTowards(
            _frameVelocity.x,
            _moveInput.x * moveSpeed,
            _stats.MoveAcceleration * Time.fixedDeltaTime
        );

    }

    #endregion

    #region JUMP

    // jump buffer
    private float _timeJumpPressed = float.MinValue;
    private bool HasBufferedJump => _time <= _timeJumpPressed + _stats.JumpBuffer;

    // coyote buffer
    private bool HasCoyoteJump => _time <= _timeLeftGround + _stats.CoyoteTime;

    // multi-jump
    private int _jumpsRemaining;

    private void HandleJump()
    {
        // this should only happen when the player leaves the ground without jumping
        if (!_col.OnGround && _jumpsRemaining == _stats.JumpCount && !HasCoyoteJump) _jumpsRemaining--;

        // prevent player from using a jump while dashing
        if (_dashing) return;

        // check if jump input is in buffer
        if (_col.OnGround && HasBufferedJump) Jump();
        // check if player pressed jump within coyote window
        else if (_jumpPressedThisFrame && !_col.OnGround && HasCoyoteJump) Jump();
        // check if in the air and player want's to multi-jump
        else if (_jumpPressedThisFrame && !_col.OnGround && _jumpsRemaining > 0) AirJump();

        _jumpPressedThisFrame = false;
    }

    private void Jump()
    {
        if (GroundJumpEffect != null) Instantiate(GroundJumpEffect, transform.position, Quaternion.identity);
        _frameVelocity.y = _stats.JumpPower;
        _timeJumpPressed = float.MinValue;
        _jumpsRemaining--;
    }

    private void AirJump()
    {
        if (AirJumpEffect != null) Instantiate(AirJumpEffect, transform.position, Quaternion.identity);
        _frameVelocity.y = _stats.AirJumpPower;
        _timeJumpPressed = float.MinValue;
        _jumpsRemaining--;
    }

    #endregion

    #region DASH

    // tracks whether the player is currently dashing
    private bool _dashing;
    // only reset to true when player is on the ground
    private bool _dashAvailable;
    // stores x velocity from before dash for smoother exit transition
    private float _cachedXVelocity;
    // tracks the time when the dash started
    private float _timeDashStarted;

    private void HandleDash()
    {
        if (_dashAvailable && (_dashPressedThisFrame || _dashing))
            Dash();

        _dashPressedThisFrame = false;
    }

    private void Dash()
    {
        if (_dashPressedThisFrame && !_dashing)
        {
            _timeDashStarted = _time;
            _dashing = true;
            _cachedXVelocity = Mathf.Abs(_frameVelocity.x);
        }
        else if (_time >= _timeDashStarted + _stats.DashTime)
        {
            _dashing = false;
            _dashDirection = Vector2.zero;
            _dashAvailable = false;
            _frameVelocity.x = _cachedXVelocity * Math.Sign(_moveInput.x);
            return;
        }

        float speed = _stats.DashDistance / _stats.DashTime;
        _frameVelocity = speed * _dashDirection;
    }

    #endregion

    #region GRAVITY

    private void HandleGravity()
    {
        if (_col.OnGround && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
            return;
        }

        float gravity = _frameVelocity.y > 0f ? _stats.RisingGravity : _stats.FallingGravity;
        if ((!_jumpHeld || _moveInput.y < 0f) && _frameVelocity.y > 0f) gravity *= _stats.EarlyJumpReleaseModifier;

        float fallSpeed = _stats.MaxFallSpeed;
        if (_moveInput.y < 0f && _frameVelocity.y < 0f) fallSpeed = _stats.QuickFallSpeed;

        _frameVelocity.y -= gravity * Time.fixedDeltaTime;
        _frameVelocity.y = Mathf.Max(_frameVelocity.y, -fallSpeed);
    }

    #endregion

    #region INPUT

    private Vector2 _moveInput;

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 ipt = context.ReadValue<Vector2>();

        ipt.x = Mathf.Abs(ipt.x) >= _stats.HorizontalDeadzone ? ipt.x : 0;
        ipt.y = Mathf.Abs(ipt.y) >= _stats.VerticalDeadzone ? ipt.y : 0;

        if (_stats.SnapInput)
        {
            ipt.x = Math.Sign(ipt.x);
            ipt.y = Math.Sign(ipt.y);
        }

        // check if player has turned
        _moveInput = ipt;
        if (ipt.x > 0f && !_isFacingRight)
        {
            Vector3 rotator = new(
                transform.rotation.x,
                0f,
                transform.rotation.z
            );
            transform.rotation = Quaternion.Euler(rotator);
            _isFacingRight = true;
        }
        else if (ipt.x < 0f && _isFacingRight)
        {
            Vector3 rotator = new(
                transform.rotation.x,
                180f,
                transform.rotation.z
            );
            transform.rotation = Quaternion.Euler(rotator);
            _isFacingRight = false;
        }
    }

    private bool _jumpHeld;
    private bool _jumpPressedThisFrame;

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _timeJumpPressed = _time;
            _jumpPressedThisFrame = true;
            _jumpHeld = true;
        }
        else if (context.canceled)
        {
            _jumpHeld = false;
        }
    }

    private bool _dashPressedThisFrame;
    private Vector2 _dashDirection;

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _dashPressedThisFrame = true;

            // TODO: Allow for up and down dashes?
            if (!_dashing) _dashDirection = _isFacingRight
                ? Vector2.right
                : Vector2.left;
        }
    }

    #endregion
}
