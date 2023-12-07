using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputInfo : MonoBehaviour
{
    public Vector2 Move => GameManager.Instance.PlayerCanMove ? _move : Vector2.zero;
    private Vector2 _move;
    public Vector2 Aim => GameManager.Instance.PlayerCanMove ? _aim : Vector2.zero;
    private Vector2 _aim;
    public bool Jump => GameManager.Instance.PlayerCanMove ? _jump : false;
    private bool _jump;
    public bool Grapple => GameManager.Instance.PlayerCanMove ? _grapple : false;
    private bool _grapple;
    public bool Attack => GameManager.Instance.PlayerCanMove ? _attack : false;
    private bool _attack;

    public bool AttackToUse => GameManager.Instance.PlayerCanMove ? _attackToUse : false;
    private bool _attackToUse;

    public bool JumpToUse => GameManager.Instance.PlayerCanMove ? _jumpToUse : false;
    private bool _jumpToUse;


    // ------------------------------------------------------

    public bool JumpPressedThisFrame => GameManager.Instance.PlayerCanMove ? _jumpPressedThisFrame : false;
    private bool _jumpPressedThisFrame;
    public bool DashPressedThisFrame => GameManager.Instance.PlayerCanMove ? _dashPressedThisFrame : false;
    private bool _dashPressedThisFrame;
    public bool GrapplePressedThisFrame => GameManager.Instance.PlayerCanMove ? _grapplePressedThisFrame : false;
    private bool _grapplePressedThisFrame;
    public bool AttackPressedThisFrame => GameManager.Instance.PlayerCanMove ? _attackPressedThisFrame : false;
    private bool _attackPressedThisFrame;

    // ------------------------------------------------------

    public float TimeJumpPressed => GameManager.Instance.PlayerCanMove ? _timeJumpPressed : float.MinValue;
    private float _timeJumpPressed;
    public float TimeDashPressed => GameManager.Instance.PlayerCanMove ? _timeDashPressed : float.MinValue;
    private float _timeDashPressed;
    public float TimeGrapplePressed => GameManager.Instance.PlayerCanMove ? _timeGrapplePressed : float.MinValue;
    private float _timeGrapplePressed;
    public float TimeGrappleReleased => GameManager.Instance.PlayerCanMove ? _timeGrappleReleased : float.MinValue;
    private float _timeGrappleReleased;
    public float TimeAttackPressed => GameManager.Instance.PlayerCanMove ? _timeAttackPressed : float.MinValue;
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
}
