using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputInfo : MonoBehaviour
{
    public Vector2 Move => GameManager.Instance.PlayerCanInput ? _move : Vector2.zero;
    private Vector2 _move;
    public Vector2 Aim => GameManager.Instance.PlayerCanInput ? _aim : Vector2.zero;
    private Vector2 _aim;
    public bool Jump => GameManager.Instance.PlayerCanInput ? _jump : false;
    private bool _jump;
    public bool Grapple => GameManager.Instance.PlayerCanInput ? _grapple : false;
    private bool _grapple;
    public bool Attack => GameManager.Instance.PlayerCanInput ? _attack : false;
    private bool _attack;

    public bool AttackToUse => GameManager.Instance.PlayerCanInput ? _attackToUse : false;
    private bool _attackToUse;

    public bool JumpToUse => GameManager.Instance.PlayerCanInput ? _jumpToUse : false;
    private bool _jumpToUse;

    public bool DashToUse => GameManager.Instance.PlayerCanInput ? _dashToUse : false;
    private bool _dashToUse;

    // ------------------------------------------------------

    public bool JumpPressedThisFrame => GameManager.Instance.PlayerCanInput ? _jumpPressedThisFrame : false;
    private bool _jumpPressedThisFrame;
    public bool DashPressedThisFrame => GameManager.Instance.PlayerCanInput ? _dashPressedThisFrame : false;
    private bool _dashPressedThisFrame;
    public bool GrapplePressedThisFrame => GameManager.Instance.PlayerCanInput ? _grapplePressedThisFrame : false;
    private bool _grapplePressedThisFrame;
    public bool AttackPressedThisFrame => GameManager.Instance.PlayerCanInput ? _attackPressedThisFrame : false;
    private bool _attackPressedThisFrame;

    // ------------------------------------------------------

    public float TimeJumpPressed => GameManager.Instance.PlayerCanInput ? _timeJumpPressed : float.MinValue;
    private float _timeJumpPressed;
    public float TimeDashPressed => GameManager.Instance.PlayerCanInput ? _timeDashPressed : float.MinValue;
    private float _timeDashPressed;
    public float TimeGrapplePressed => GameManager.Instance.PlayerCanInput ? _timeGrapplePressed : float.MinValue;
    private float _timeGrapplePressed;
    public float TimeGrappleReleased => GameManager.Instance.PlayerCanInput ? _timeGrappleReleased : float.MinValue;
    private float _timeGrappleReleased;
    public float TimeAttackPressed => GameManager.Instance.PlayerCanInput ? _timeAttackPressed : float.MinValue;
    private float _timeAttackPressed;

    private PlayerStats GameSettings => GameManager.Instance.PlayerStats;
    private float _time;

    private void Start()
    {
        _timeJumpPressed = float.MinValue;
        _timeDashPressed = float.MinValue;
        _timeGrapplePressed = float.MinValue;
    }

    private void Update()
    {
        _time += Time.deltaTime;

        _jumpPressedThisFrame = false;
        _dashPressedThisFrame = false;
        _grapplePressedThisFrame = false;
        _attackPressedThisFrame = false;

        if (_time > TimeAttackPressed + GameManager.Instance.PlayerStats.AttackInputBufferTime)
            _attackToUse = false;

        if (_time > TimeJumpPressed + GameManager.Instance.PlayerStats.JumpBuffer)
            _jumpToUse = false;

        if (_time > TimeDashPressed + GameManager.Instance.PlayerStats.DashInputBufferTime)
            _dashToUse = false;
    }

    /// <summary>
    /// Handles the movement input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 ipt = context.ReadValue<Vector2>();

        _aim = ipt.magnitude > GameSettings.AimDeadzone ? ipt.normalized : Vector2.zero;

        ipt.x = Mathf.Abs(ipt.x) >= GameSettings.HorizontalDeadzone ? ipt.x : 0;
        ipt.y = Mathf.Abs(ipt.y) >= GameSettings.VerticalDeadzone ? ipt.y : 0;

        if (GameSettings.SnapInput)
        {
            ipt.x = Math.Sign(ipt.x);
            ipt.y = Math.Sign(ipt.y);
        }

        _move = ipt;
    }

    /// <summary>
    /// Handles the jump input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jump = true;
            _jumpPressedThisFrame = true;
            _timeJumpPressed = _time;
            _jumpToUse = true;
        }
        else if (context.canceled)
        {
            _jump = false;
        }
    }

    /// <summary>
    /// Handles the dash input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _dashPressedThisFrame = true;
            _timeDashPressed = _time;
            _dashToUse = true;
        }
    }

    /// <summary>
    /// Handles the dash input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
    public void OnGrapple(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _grapple = true;
            _grapplePressedThisFrame = true;
            _timeGrapplePressed = _time;
        }
        else if (context.canceled)
        {
            _grapple = false;
            _timeGrappleReleased = _time;
        }
    }

    /// <summary>
    /// Handles the attack input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _attackPressedThisFrame = true;
            _attackToUse = true;
            _timeAttackPressed = _time;
            _attack = true;
        }
        else if (context.canceled)
            _attack = false;
    }

    public void UseAttack() => _attackToUse = false;
    public void UseJump() => _jumpToUse = false;
    public void UseDash() => _dashToUse = false;
}
