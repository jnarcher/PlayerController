using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerCollision))]
public class PlayerController : MonoBehaviour
{
    // Component references
    private Rigidbody2D _rb;
    private PlayerCollision _col;
    private PlayerGrapple _playerGrapple;
    private PlayerStats Stats => GameManager.Instance.PlayerStats;

    public GameObject GrappleDirectionIndicator;

    // Effects
    [Header("Effects")]
    public GameObject LandEffect;
    public GameObject GroundJumpEffect;
    public GameObject AirJumpEffect;
    public GameObject WallJumpEffect;
    public GameObject DashEffect;

    #region INTERFACE

    public Vector2 Position => _rb.position;
    public Vector2 Velocity => _frameVelocity;
    public bool IsFacingRight => _isFacingRight;
    public bool IsAimingGrapple => _aimingGrapple;

    #endregion

    private float _time;
    private Vector2 _frameVelocity;
    private bool _isFacingRight = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<PlayerCollision>();
        _playerGrapple = GetComponent<PlayerGrapple>();
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
        HandleGrapple();
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

    /// <summary>
    /// Collects collision data from player collision class and sets the necessary variables.
    /// </summary>
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
            ResetDash();
        }

        if (_col.OnGround)
        {
            ResetAirJumps();
            _dashAvailable = _time > _timeDashEnded + Stats.GroundDashCooldown;
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
    }

    #endregion

    #region HORIZONTAL

    private float _cachedMoveInputX;

    /// <summary>
    /// This will apply the horizontal velocity of the character.
    /// </summary>
    private void HandleHorizontal()
    {
        if (_grapplePressedThisFrame)
            _cachedMoveInputX = _moveInput.x;
        _grapplePressedThisFrame = false;

        float moveAcceleration = Stats.MoveAcceleration;

        if (!_col.OnGround && Mathf.Abs(_frameVelocity.y) < Stats.JumpApexWindow)
            moveAcceleration *= Stats.JumpApexMoveAccelerationMultiplier;

        // Lerp acceleration after wall jumping
        float x = 1;
        if (!_col.OnGround && _time <= _timeWallJumped + Stats.WallJumpInputFreezeTime)
            x = Mathf.Lerp(0, 1, (_time - _timeWallJumped) / Stats.WallJumpInputFreezeTime);
        else if (!_col.OnGround && _time <= _timeStoppedGrappling + Stats.GrappleInputFreezeTime)
            x = Mathf.Lerp(0, 1, (_time - _timeStoppedGrappling) / Stats.GrappleInputFreezeTime);

        float moveInputX = _moveInput.x;
        if (_aimingGrapple) moveInputX = _cachedMoveInputX;

        _frameVelocity.x = Mathf.MoveTowards(
            _frameVelocity.x,
            moveInputX * Stats.MoveSpeed,
            x * moveAcceleration * Time.fixedDeltaTime
        );
    }

    #endregion

    #region JUMP

    // jump buffer
    private float _timeJumpPressed = float.MinValue;
    private bool HasBufferedJump => _time <= _timeJumpPressed + Stats.JumpBuffer;

    // coyote buffer
    private bool HasCoyoteJump => _time <= _timeLeftGround + Stats.CoyoteTime;

    // multi-jump
    private int _airJumpsRemaining;

    // wall jumps
    private float _timeWallJumped = float.MinValue;
    private bool _lastWallWasRight;
    private bool HasBufferedWallJump => _time <= _timeLeftWall + Stats.WallJumpBuffer;

    /// <summary>
    /// Determines whether the player can jump.
    /// </summary>
    private void HandleJump()
    {
        if (_dashing) return; // can't jump while dashing

        if (_col.OnGround && HasBufferedJump) Jump();
        else if (_jumpPressedThisFrame && (HasBufferedWallJump || _col.OnWall)) WallJump();
        else if (_jumpPressedThisFrame && !_col.OnGround && HasCoyoteJump) Jump();
        else if (_jumpPressedThisFrame && !_col.OnGround && _airJumpsRemaining > 0) AirJump();

        _jumpPressedThisFrame = false;
    }

    /// <summary>
    /// Apply the velocity for a ground jump.
    /// </summary>
    private void Jump()
    {
        if (GroundJumpEffect != null) Instantiate(GroundJumpEffect, transform.position, Quaternion.identity);
        _frameVelocity.y = Stats.JumpPower;
        _timeJumpPressed = float.MinValue;
    }

    /// <summary>
    /// Apply the velocity for an air jump.
    /// </summary>
    private void AirJump()
    {
        if (AirJumpEffect != null) Instantiate(AirJumpEffect, transform.position, Quaternion.identity);
        _frameVelocity.y = Stats.AirJumpPower;
        _timeJumpPressed = float.MinValue;
        _airJumpsRemaining--;
    }

    private void ResetAirJumps() => _airJumpsRemaining = Stats.AirJumpCount;

    /// <summary>
    /// Apply the velocity for a wall jump.
    /// </summary>
    private void WallJump()
    {
        if (WallJumpEffect != null) Instantiate(WallJumpEffect, transform.position, Quaternion.identity);
        _frameVelocity.y = Stats.WallJumpVelocity.y;
        _frameVelocity.x = Stats.WallJumpVelocity.x;
        if (_lastWallWasRight) _frameVelocity.x *= -1;
        ResetAirJumps();
        ResetDash();
        _timeWallJumped = _time;
    }

    #endregion

    #region DASH

    private bool _dashing;
    private bool _dashAvailable;
    private float _cachedXVelocity;
    private float _timeDashStarted;
    private float _timeDashEnded = float.MinValue;

    /// <summary>
    /// Determines whether the player can dash.
    /// </summary>
    private void HandleDash()
    {
        // check if player can dash in the air
        if ((!_col.OnGround && !Stats.AirDashToggle || _col.OnWall) && !_dashing)
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
        else if (_dashing && _time >= _timeDashStarted + Stats.DashTime)
        {
            _dashing = false;
            _dashDirection = Vector2.zero;
            _dashAvailable = false;
            _timeDashEnded = _time;
            _frameVelocity.x = _cachedXVelocity * Mathf.Sign(_moveInput.x);
        }

        if (_dashing && Stats.DashToggle) Dash();

        _dashPressedThisFrame = false;
    }

    /// <summary>
    /// Applies the correct velocity for a dash.
    /// </summary>
    private void Dash()
    {
        float speed = Stats.DashDistance / Stats.DashTime;
        _frameVelocity = speed * _dashDirection;
    }

    private void ResetDash() => _dashAvailable = Stats.AirDashToggle;

    #endregion

    #region GRAPPLE

    private float _timeStartedAimingGrapple = float.MinValue;
    private float _timeStoppedGrappling = float.MinValue;
    private bool _grappling;
    private GameObject _pointGrapplingTo;

    private void HandleGrapple()
    {
        if (_aimingGrapple && !_grappling)
            Time.timeScale = Mathf.Lerp(1f, Stats.GrappleTimeSlow, (_time - _timeStartedAimingGrapple) / Stats.GrappleTimeSlowSpeed);
        else
            Time.timeScale = 1f;

        if (_grappleReleasedThisFrame)
        {
            GameObject hitGrapplePoint = _playerGrapple.FindGrappleFromInput(_grappleAimInput);
            if (hitGrapplePoint != null)
            {
                _pointGrapplingTo = hitGrapplePoint;
                _grappling = true;
            }
        }

        if (_grappling) Grapple(_pointGrapplingTo.transform.position);

        _grappleReleasedThisFrame = false;
    }

    private void Grapple(Vector3 point)
    {
        if (Vector2.Distance(transform.position, point) <= Stats.GrappleStopDistance)
        {
            _grappling = false;
            _timeStoppedGrappling = _time;
            ResetAirJumps();
            ResetDash();
            return;
        }
        Vector2 grapplePointDirection = (point - transform.position).normalized;
        _frameVelocity = Stats.GrappleSpeed * grapplePointDirection;
    }

    #endregion

    #region GRAVITY

    /// <summary>
    /// Applies downward acceleration to the player.
    /// </summary>
    private void HandleGravity()
    {
        if (_col.OnGround && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = Stats.GroundingForce;
            return;
        }

        // set the correct gravity
        float gravity = _frameVelocity.y > 0f ? Stats.RisingGravity : Stats.FallingGravity;
        if (!_jumpHeld && _frameVelocity.y > 0f)
            gravity *= Stats.EarlyJumpReleaseModifier;
        else if (!_col.OnGround && Mathf.Abs(_frameVelocity.y) < Stats.JumpApexWindow)
            gravity *= Stats.JumpApexGravityMultiplier;

        float maxFallSpeed = Stats.MaxFallSpeed;
        if (Stats.WallSlideJumpToggle && _col.OnWall)
            maxFallSpeed = Stats.WallSlideSpeed;

        _frameVelocity.y -= gravity * Time.fixedDeltaTime;
        _frameVelocity.y = Mathf.Max(_frameVelocity.y, -maxFallSpeed);
    }

    #endregion

    #region INPUT

    private Vector2 _moveInput;
    private Vector2 _grappleAimInput;

    /// <summary>
    /// Handles the movement input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 ipt = context.ReadValue<Vector2>();

        _grappleAimInput = ipt;

        ipt.x = Mathf.Abs(ipt.x) >= Stats.HorizontalDeadzone ? ipt.x : 0;
        ipt.y = Mathf.Abs(ipt.y) >= Stats.VerticalDeadzone ? ipt.y : 0;

        if (Stats.SnapInput)
        {
            ipt.x = Math.Sign(ipt.x);
            ipt.y = Math.Sign(ipt.y);
        }

        _moveInput = ipt;

        if (!_dashing && !_aimingGrapple)
        {
            // check if player has turned
            if (ipt.x > 0f && !_isFacingRight)
            {
                transform.localEulerAngles = new Vector3(
                    transform.rotation.x,
                    0f,
                    transform.rotation.z
                );
                _isFacingRight = true;
            }
            else if (ipt.x < 0f && _isFacingRight)
            {
                transform.localEulerAngles = new Vector3(
                    transform.rotation.x,
                    180f,
                    transform.rotation.z
                );
                _isFacingRight = false;
            }
        }
    }

    private bool _jumpHeld;
    private bool _jumpPressedThisFrame;

    /// <summary>
    /// Handles the jump input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
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

    /// <summary>
    /// Handles the dash input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
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

    private bool _aimingGrapple = false;
    private bool _grapplePressedThisFrame;
    private bool _grappleReleasedThisFrame;

    public void OnGrapple(InputAction.CallbackContext context)
    {
        if (!Stats.GrappleToggle)
        {
            _aimingGrapple = false;
            return;
        }

        if (context.performed)
        {
            _aimingGrapple = true;
            _timeStartedAimingGrapple = _time;
            _grapplePressedThisFrame = true;
            GrappleDirectionIndicator.SetActive(true);
        }
        else if (context.canceled)
        {
            _aimingGrapple = false;
            _grappleReleasedThisFrame = true;
            GrappleDirectionIndicator.SetActive(false);
        }
    }

    #endregion
}
