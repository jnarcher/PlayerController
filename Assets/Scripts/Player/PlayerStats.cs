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
    [Tooltip("Sets the minimum input magnitude (in a direction) that registers as input for aiming."), Range(0f, 0.99f)]
    public float AimDeadzone = 0.5f;

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
    public float GroundingForce = 1.5f;

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

    [Header("Wall Slide / Jump")]
    [Tooltip("Toggle whether the player can wall slide and jump or not.")]
    public bool WallSlideJumpToggle = true;
    [Tooltip("The speed the player falls down walls.")]
    public float WallSlideSpeed = 10f;
    [Tooltip("The velocity that gets applied when the player wall jumps.")]
    public Vector2 WallJumpVelocity = new(20f, 20f);
    [Tooltip("The time after a player has left the wall where they can still wall jump. (Similar to coyote time).")]
    public float WallJumpBuffer = 0.2f;
    [Tooltip("The amount of time the players input is lerped after a wall jump.")]
    public float WallJumpInputFreezeTime = 0.2f;

    [Header("Dash")]
    [Tooltip("Toggle whether the player can dash or not.")]
    public bool DashToggle = true;
    [Tooltip("Toggle whether the player can dash in the air or not.")]
    public bool AirDashToggle = true;
    [Tooltip("How quickly it takes a dash to reach the dash distance.")]
    public float DashTime = 1f;
    [Tooltip("The maximum distance a dash can take the player horizontally.")]
    public float DashDistance = 4f;
    [Tooltip("How long the player has to wait before dashing again after a dash has been performed while on the ground.")]
    public float GroundDashCooldown = 0.5f;

    [Header("Grapple")]
    [Tooltip("Toogle whether the player can grapple.")]
    public bool GrappleToggle = true;
    [Tooltip("The speed at which the grapple launches you.")]
    public float GrappleSpeed = 30f;
    [Tooltip("The range of the grapple.")]
    public float GrappleRange = 10f;
    [Tooltip("How much time slows while aiming the grapple."), Range(0.01f, 1f)]
    public float GrappleTimeSlow = 0.2f;
    [Tooltip("The speed the time slow is interpolated with.")]
    public float GrappleTimeSlowTransitionSpeed = 0.1f;
    [Tooltip("The distance from the player to the grapple point at which the grapple will detach.")]
    public float GrappleStopDistance = 0.5f;
    [Tooltip("The amount of time the input is lerped after a grapple finishes (similar to wall jump).")]
    public float GrappleInputFreezeTime = 0.2f;
    [Tooltip("The range in which the grapple point can be within around the aim input for the grapple to execute successfully. (Aim assist)")]
    public float GrappleAssistAngle = 45f;

    [Header("Health")]
    [Tooltip("The maximum number of health the player can have.")]
    public int MaxHealth = 5;

    [Header("Ground Attacks")]
    [Tooltip("The amount of damage a ground attack does to enemies.")]
    public int GroundAttackDamage = 1;
    [Tooltip("The amount of time between ground attacks.")]
    public float GroundAttackCooldown = 0.2f;
    [Tooltip("The amount of knockback enemies take from being hit by the first ground attacks.")]
    public float GroundAttack1KnockbackStrength = 15f;
    [Tooltip("How long in seconds is the first ground attack")]
    public float GroundAttack1Length = 0.2f;
    [Tooltip("The strength of the movement during the player's first ground attack")]
    public float GroundAttack1MovementStrength = 10f;
    [Tooltip("The movement of the player during the player's first ground attack")]
    public AnimationCurve GroundAttack1MovementCurve;
    [Tooltip("The amount of knockback enemies take from being hit by the second ground attack combo.")]
    public float GroundAttack2KnockbackStrength = 30f;
}