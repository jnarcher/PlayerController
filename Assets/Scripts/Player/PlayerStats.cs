using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Input")]
    [Tooltip("Forces movement input to integer values, movement won't scale with analouge input.")]
    public bool SnapInput = true;
    [Tooltip("Sets the minimum of vertical input for movement that is needed to register."), Range(0f, 0.99f)]
    public float HorizontalDeadzone = 0f;
    [Tooltip("Sets the minimum of vertical input for movement that is needed to register."), Range(0f, 0.99f)]
    public float VerticalDeadzone = 0f;

    [Header("Horizontal Movement")]
    [Tooltip("Maximum horizontal move speed.")]
    public float MoveSpeed = 10f;
    [Tooltip("Rate at which the move speed changes.")]
    public float MoveAcceleration = 200f;
    [Tooltip("Maximum speed the player falls at.")]
    public float MaxFallSpeed = 40f;
    [Tooltip("Extra downward acceleration if the player inputs the down movement direction.")]
    public float QuickFallSpeed = 50f;
    [Tooltip("Gravity while travelling up through the air.")]
    public float FallingGravity = 400f;
    [Tooltip("Gravity while travelling down through the air.")]
    public float RisingGravity = 300f;
    [Tooltip("A contsant force while on the ground. (Good for slopes)")]
    public float GroundingForce = -1.5f;

    [Header("Jump")]
    [Tooltip("Vertical velocity of a jump.")]
    public float JumpPower = 20f;
    [Tooltip("Vertical velocity of a mid-air jump.")]
    public float AirJumpPower = 10f;
    [Tooltip("The amount of jumps the player can use in the air.")]
    public int AirJumpCount = 1;
    [Tooltip("An extra downforce when the player releases the jump button.")]
    public float EarlyJumpReleaseModifier = 2f;

    [Header("Jump Apex")]
    [Tooltip("The y-velocity range in which the player is considered in the apex of the jump arc.")]
    public float JumpApexWindow = 5f;
    [Tooltip("The gravity multiplier while at the apex of a jump"), Range(0.01f, 1f)]
    public float JumpApexGravityMultiplier = 0.5f;
    [Tooltip("The increase in horizontal speed at the apex of a jump.")]
    public float JumpApexMoveAccelerationMultiplier = 2f;

    [Header("Jump Buffering")]
    [Tooltip("The amount of time in seconds that the player can press jump before hitting the ground and still have it register.")]
    public float JumpBuffer = 0.2f;

    [Header("Coyote Buffer")]
    [Tooltip("The amount of time in seconds that the player can still jump after leaving the ground.")]
    public float CoyoteTime = 0.15f;

    [Header("Dash")]
    [Tooltip("Toggle whether the player can dash or not.")]
    public bool DashToggle = true;
    [Tooltip("How quickly it takes a dash to reach the dash distance.")]
    public float DashTime = 1f;
    [Tooltip("The maximum distance a dash can take the player horizontally.")]
    public float DashDistance = 4f;
    [Tooltip("How long the player has to wait before dashing again after a dash has been performed while on the ground.")]
    public float GroundDashCooldown = 0.5f;
}
