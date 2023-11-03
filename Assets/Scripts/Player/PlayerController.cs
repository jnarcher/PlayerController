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
    public GameObject WallJumpEffect;
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
    private bool _onWall;
    private float _timeLeftGround = float.MinValue;
    private float _timeLeftWall = float.MinValue;

    private void HandleCollisions()
    {
        if (_col.OnCeiling)
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        if (!_grounded && _col.OnGround)
            OnLanding();
        else if (_grounded && !_col.OnGround)
        {
            _timeLeftGround = _time;
            // the player will always get their dash back immediately after leaving the ground (ignoring cooldown)
            _dashAvailable = _stats.AirDashToggle;
        }

        if (_col.OnGround)
        {
            _airJumpsRemaining = _stats.AirJumpCount;
            _dashAvailable = _time > _timeDashEnded + _stats.GroundDashCooldown;
        }

        if (_col.OnWall)
        {
            _lastWallWasRight = _isFacingRight;
        }
        else if (_onWall && !_col.OnWall)
        {
            _timeLeftWall = _time;
        }

        _onWall = _col.OnWall;
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
        if (_time <= _timeWallJumped + _stats.WallJumpInputFreezeTime) return;

        float moveAcceleration = _stats.MoveAcceleration;

        if (!_col.OnGround && Mathf.Abs(_frameVelocity.y) < _stats.JumpApexWindow)
            moveAcceleration *= _stats.JumpApexMoveAccelerationMultiplier;

        // Lerp input after wall jumping
        float xInput = _moveInput.x;
        if (_time <= _timeWallJumped + _stats.WallJumpInputFreezeTime)
            xInput *= Mathf.Lerp(0, 1, (_timeWallJumped + _time) / (_timeWallJumped + _stats.WallJumpInputFreezeTime));

        _frameVelocity.x = Mathf.MoveTowards(
            _frameVelocity.x,
            xInput * _stats.MoveSpeed,
            moveAcceleration * Time.fixedDeltaTime
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
    private int _airJumpsRemaining;

    // wall jumps
    private float _timeWallJumped = float.MinValue;
    private bool _lastWallWasRight;
    private bool HasBufferedWallJump => _time <= _timeLeftWall + _stats.WallJumpBuffer;

    private void HandleJump()
    {
        if (_dashing) return; // can't jump while dashing

        if (_col.OnGround && HasBufferedJump) Jump();
        else if (_jumpPressedThisFrame && (HasBufferedWallJump || _col.OnWall)) WallJump();
        else if (_jumpPressedThisFrame && !_col.OnGround && HasCoyoteJump) Jump();
        else if (_jumpPressedThisFrame && !_col.OnGround && _airJumpsRemaining > 0) AirJump();

        _jumpPressedThisFrame = false;
    }

    private void Jump()
    {
        if (GroundJumpEffect != null) Instantiate(GroundJumpEffect, transform.position, Quaternion.identity);
        _frameVelocity.y = _stats.JumpPower;
        _timeJumpPressed = float.MinValue;
    }

    private void AirJump()
    {
        if (AirJumpEffect != null) Instantiate(AirJumpEffect, transform.position, Quaternion.identity);
        _frameVelocity.y = _stats.AirJumpPower;
        _timeJumpPressed = float.MinValue;
        _airJumpsRemaining--;
    }

    private void WallJump()
    {
        if (WallJumpEffect != null) Instantiate(WallJumpEffect, transform.position, Quaternion.identity);
        _frameVelocity.y = _stats.WallJumpVelocity.y;
        _frameVelocity.x = _stats.WallJumpVelocity.x;
        if (_lastWallWasRight) _frameVelocity.x *= -1;
        _airJumpsRemaining = _stats.AirJumpCount;
        _dashAvailable = _stats.AirDashToggle;
        _timeWallJumped = _time;
    }

    #endregion

    #region DASH

    private bool _dashing;
    private bool _dashAvailable;
    private float _cachedXVelocity;
    private float _timeDashStarted;
    private float _timeDashEnded = float.MinValue;

    private void HandleDash()
    {
        // check if player can dash in the air
        if (!_col.OnGround && !_stats.AirDashToggle && !_dashing)
        {
            _dashPressedThisFrame = false;
            return;
        }

        // start a dash
        if (_dashPressedThisFrame && _dashAvailable && !_dashing)
        {
            _timeDashStarted = _time;
            _dashing = true;
            _cachedXVelocity = Mathf.Abs(_frameVelocity.x);
        }
        // end a dash
        else if (_dashing && _time >= _timeDashStarted + _stats.DashTime)
        {
            _dashing = false;
            _dashDirection = Vector2.zero;
            _dashAvailable = false;
            _timeDashEnded = _time;
            _frameVelocity.x = _cachedXVelocity * Mathf.Sign(_moveInput.x);
        }

        if (_dashing && _stats.DashToggle) Dash();

        _dashPressedThisFrame = false;
    }

    private void Dash()
    {
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

        // set the correct gravity
        float gravity = _frameVelocity.y > 0f ? _stats.RisingGravity : _stats.FallingGravity;
        if ((!_jumpHeld || _moveInput.y < 0f) && _frameVelocity.y > 0f)
            gravity *= _stats.EarlyJumpReleaseModifier;
        else if (!_col.OnGround && Mathf.Abs(_frameVelocity.y) < _stats.JumpApexWindow)
            gravity *= _stats.JumpApexGravityMultiplier;

        // set the correct maximum fall speed
        float maxFallSpeed = _stats.MaxFallSpeed;
        if (_moveInput.y < 0f && _frameVelocity.y < 0f)
            maxFallSpeed = _stats.QuickFallSpeed;

        // check for wall sliding
        if (_stats.WallSlideJumpToggle && _col.OnWall)
            maxFallSpeed = _stats.WallSlideSpeed;

        _frameVelocity.y -= gravity * Time.fixedDeltaTime;
        _frameVelocity.y = Mathf.Max(_frameVelocity.y, -maxFallSpeed);
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
