using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    /// <summary>
    /// This handles the logic for a dash.
    /// </summary>
    public class DashState : PlayerState
    {
        private float _dashStartTime;
        private float _dashDirection;
        private float _cachedXSpeed;
        private float DashSpeed => Stats.DashDistance / Stats.DashTime;
        private bool _hitWall;

        private bool _isSlide;

        public DashState(Player player) : base(player) { }

        public override void EnterState()
        {
            _dashStartTime = Player.ElapsedTime;
            _cachedXSpeed = Mathf.Abs(Player.Velocity.x);

            // Dash goes in direction of input rather than player facing direction
            if (InputInfo.Move.x == 0)
                _dashDirection = Player.IsFacingRight ? 1 : -1;
            else
                _dashDirection = InputInfo.Move.x > 0 ? 1 : -1;

            _hitWall = false;

            // if on wall, dash away from the wall
            if (TriggerInfo.OnWall)
            {
                _dashDirection *= -1;
                _cachedXSpeed = DashSpeed;
                Player.ResetAirJumps();
            }

            Player.SetFacing(_dashDirection > 0);
            Player.SetVelocity(_dashDirection * DashSpeed, 0);
            Player.SetGravity(0);

            _isSlide = TriggerInfo.OnGround;

            if (_isSlide)
            {
                Player.Animator.SetBool("Sliding", true);
                Player.SlideAttackHitbox.enabled = true;
            }
            else
                Player.Animator.SetBool("Dashing", true);

            Player.GiveInvincibility(200f); // TODO: Think of better implementation
        }

        public override void UpdateState()
        {
            if (Player.ElapsedTime >= _dashStartTime + Stats.DashTime)
                Player.SetState(PlayerStateType.Move);

            if (_isSlide)
                DealDamage();
        }

        public override void FixedUpdateState()
        {
            if (TriggerInfo.HitWallThisFrame && !TriggerInfo.OnGround)
                _hitWall = true;
        }

        public override void ExitState()
        {
            Player.SetGravity(Stats.RisingGravity);
            Player.SetVelocity(_dashDirection * _cachedXSpeed, 0);
            if (_hitWall)
                Player.ResetDash();
            else
                Player.SetDashCooldown();

            Player.Animator.SetBool("Dashing", false);
            Player.Animator.SetBool("Sliding", false);

            Player.SlideAttackHitbox.enabled = false;

            Player.StopInvincibility();
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(Player.SlideAttackHitbox);
            foreach (var enemy in enemies)
            {
                enemy.Damage(
                    0,
                    new Vector2(
                        _dashDirection * Stats.SlideAttackKnockback.x,
                        Stats.SlideAttackKnockback.y
                    )
                );
            }
        }
    }
}
