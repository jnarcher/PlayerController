using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputInfo : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Aim { get; private set; }
    public bool Jump { get; private set; }
    public bool Grapple { get; private set; }
    public bool Attack { get; private set; }

    public bool JumpPressedThisFrame { get; private set; }
    public bool DashPressedThisFrame { get; private set; }
    public bool GrapplePressedThisFrame { get; private set; }
    public bool AttackPressedThisFrame { get; private set; }

    public float TimeJumpPressed { get; private set; }
    public float TimeDashPressed { get; private set; }
    public float TimeGrapplePressed { get; private set; }
    public float TimeGrappleReleased { get; private set; }
    public float TimeAttackPressed { get; private set; }

    private PlayerStats GameSettings => GameManager.Instance.PlayerStats;
    private float _time;

    private void Start()
    {
        TimeJumpPressed = float.MinValue;
        TimeDashPressed = float.MinValue;
        TimeGrapplePressed = float.MinValue;
    }

    private void Update()
    {
        _time += Time.deltaTime;

        JumpPressedThisFrame = false;
        DashPressedThisFrame = false;
        GrapplePressedThisFrame = false;
        AttackPressedThisFrame = false;
    }

    /// <summary>
    /// Handles the movement input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 ipt = context.ReadValue<Vector2>();

        Aim = ipt.magnitude > GameSettings.AimDeadzone ? ipt.normalized : Vector2.zero;

        ipt.x = Mathf.Abs(ipt.x) >= GameSettings.HorizontalDeadzone ? ipt.x : 0;
        ipt.y = Mathf.Abs(ipt.y) >= GameSettings.VerticalDeadzone ? ipt.y : 0;

        if (GameSettings.SnapInput)
        {
            ipt.x = Math.Sign(ipt.x);
            ipt.y = Math.Sign(ipt.y);
        }

        Move = ipt;
    }

    /// <summary>
    /// Handles the jump input event sent from the `Player Input` component.
    /// </summary>
    /// <param name="context">The wrapper around the input event.</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Jump = true;
            JumpPressedThisFrame = true;
            TimeJumpPressed = _time;
        }
        else if (context.canceled)
        {
            Jump = false;
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
            DashPressedThisFrame = true;
            TimeDashPressed = _time;
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
            Grapple = true;
            GrapplePressedThisFrame = true;
            TimeGrapplePressed = _time;
        }
        else if (context.canceled)
        {
            Grapple = false;
            TimeGrappleReleased = _time;
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
            AttackPressedThisFrame = true;
            TimeAttackPressed = _time;
            Attack = true;
        }
        else if (context.canceled)
        {
            Attack = false;
        }
    }
}
