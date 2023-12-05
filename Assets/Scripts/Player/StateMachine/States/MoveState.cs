using UnityEngine;

namespace PlayerStateMachine
{
    /// <summary>
    /// This state handles all of the non-ability based movement: running, jumping, air jumping, wall sliding, wall jumping
    /// </summary>
    public class MoveState : PlayerState
    {
        // This is needed since we get the input in Update but want to use it in FixedUpdate.
        // The update function of the InputInfo might reset the JumpPressedThisFrame variable before the FixedUpdate
        // in this class is run
        private bool _jumpPressedThisFrame;

        private GameObject _wallSlideEffect;
        private ParticleSystem _wallSlideParticles;

        public MoveState(Player player) : base(player) { }

        public override void UpdateState()
        {
            if (InputInfo.JumpPressedThisFrame) _jumpPressedThisFrame = true;
            CheckStateTransitions();
        }

        public override void FixedUpdateState()
        {
            HandleCollision();
            HandleMovement();
            HandleTurn();
            HandleJump();
            HandleGravity();
        }

        public override void ExitState()
        {
            _wallSlideParticles?.Stop();
        }

        private void HandleCollision()
        {
            if (TriggerInfo.LandedThisFrame)
            {
                Player.ResetAirJumps();
                if (Player.LandingEffect != null)
                    Object.Instantiate(Player.LandingEffect, Player.Position, Quaternion.identity);
            }
            else if (TriggerInfo.LeftGroundThisFrame)
                Player.ResetDash();

            if (TriggerInfo.HitWallThisFrame)
            {
                Player.ResetDash();
                Player.ResetAttack();
                if (Player.WallSlideEffect != null)
                {
                    if (_wallSlideEffect == null)
                    {
                        _wallSlideEffect = GameObject.Instantiate(Player.WallSlideEffect, Player.Position, Quaternion.identity);
                        _wallSlideEffect.transform.parent = Player.gameObject.transform;
                        _wallSlideParticles = _wallSlideEffect.GetComponentInChildren<ParticleSystem>();
                    }
                    _wallSlideParticles?.Play();
                }
            }

            if (TriggerInfo.OnWall)
                Player.SetLastWallDirection(Player.IsFacingRight);
            else
                _wallSlideParticles?.Stop();

            Player.SetFallSpeed(
                Stats.WallSlideJumpToggle && TriggerInfo.OnWall
                    ? Stats.WallSlideSpeed
                    : Stats.MaxFallSpeed
            );
        }

        private void HandleMovement()
        {
            float acceleration = Stats.MoveAcceleration;

            float x = Mathf.Lerp(0, 1, Player.CurrentMovementLerpValue);
            float newXVelocity = Mathf.MoveTowards(
                Player.Velocity.x,
                InputInfo.Move.x * Stats.MoveSpeed,
                x * acceleration * Time.fixedDeltaTime
            );
            Player.SetVelocity(newXVelocity, Player.Velocity.y);
        }


        private void HandleTurn()
        {
            if (Player.IsFacingRight && Player.Velocity.x < 0) Player.SetFacing(false);
            else if (!Player.IsFacingRight && Player.Velocity.x > 0) Player.SetFacing(true);
        }

        #region JUMPING

        private bool HasBufferedJump => Player.ElapsedTime < InputInfo.TimeJumpPressed + Stats.JumpBuffer;
        private bool WithinCoyoteBuffer => Player.ElapsedTime < TriggerInfo.TimeLeftGround + Stats.CoyoteTime;
        private bool HasBufferedWallJump => Player.ElapsedTime < TriggerInfo.TimeLeftWall + Stats.WallJumpBuffer;
        private void HandleJump()
        {
            if (TriggerInfo.OnGround && HasBufferedJump)
                GroundJump();
            else if (_jumpPressedThisFrame)
            {
                if (Stats.WallSlideJumpToggle && (HasBufferedWallJump || TriggerInfo.OnWall))
                    WallJump();
                else if (WithinCoyoteBuffer)
                    GroundJump();
                else if (!TriggerInfo.OnGround && Player.AirJumpsRemaining > 0)
                    AirJump();
            }

            _jumpPressedThisFrame = false;
        }

        private void GroundJump()
        {
            Player.SetVelocity(Player.Velocity.x, Stats.JumpPower);

            if (Player.GroundJumpEffect != null)
                Object.Instantiate(Player.GroundJumpEffect, Player.Position, Quaternion.identity);
        }

        private void AirJump()
        {
            if (Player.AirJumpEffect != null)
                Object.Instantiate(Player.AirJumpEffect, Player.Position, Quaternion.identity);
            Player.SetVelocity(Player.Velocity.x, Stats.JumpPower);
            Player.DecrementAirJump();
        }

        private void WallJump()
        {
            if (Player.WallJumpEffect != null)
                Object.Instantiate(Player.WallJumpEffect, Player.Position, Quaternion.identity);
            Player.SetVelocity(-(Player.LastWallRight ? 1 : -1) * Stats.WallJumpVelocity.x, Stats.WallJumpVelocity.y);
            Player.ResetAirJumps();
            Player.LerpMoveAcceleration(Stats.WallJumpInputFreezeTime);
        }

        #endregion

        private void HandleGravity()
        {
            if (TriggerInfo.OnGround)
            {
                Player.SetGravity(Stats.GroundingForce);
                return;
            }

            float newGravity = Player.Velocity.y >= 0 ? Stats.RisingGravity : Stats.FallingGravity;
            if (!InputInfo.Jump && Player.Velocity.y > 0)
                newGravity *= Stats.EarlyJumpReleaseModifier;
            else if (!TriggerInfo.OnGround && Mathf.Abs(Player.Velocity.y) <= Stats.JumpApexWindow)
                newGravity *= Stats.JumpApexGravityMultiplier;

            Player.SetGravity(newGravity);
        }

        private void CheckStateTransitions()
        {
            if (Stats.DashToggle && Player.DashAvailable && InputInfo.DashPressedThisFrame && (TriggerInfo.OnGround || Stats.AirDashToggle))
                Player.SetState(PlayerStateType.Dash);
            else if (Stats.GrappleToggle && InputInfo.Grapple)
                Player.SetState(PlayerStateType.GrappleAim);
            else if (InputInfo.AttackUsable && Player.AttackOffCooldown)
                HandleAttackTransition();
        }

        private void HandleAttackTransition()
        {
            if (!TriggerInfo.OnGround && InputInfo.Move.y == -1)
                Player.SetState(PlayerStateType.DownAttack);
            else if (InputInfo.Move.y == 1)
                Player.SetState(PlayerStateType.UpAttack);
            else if (TriggerInfo.OnGround)
                Player.SetState(PlayerStateType.GroundAttack1);
            else if (!TriggerInfo.OnGround && !TriggerInfo.OnWall)
                Player.SetState(PlayerStateType.AirAttack1);
        }
    }
}