using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    /// <summary>
    /// This handles the logic for a dash.
    /// </summary>
    public class DashState : PlayerState
    {
        private float _dashTimer;
        private float _dashDirection;
        private float _cachedXSpeed;

        private float DashSpeed => Stats.DashDistance / Stats.DashTime;
        private bool _hitWall;

        private bool _isSlide;

        public DashState(Player player) : base(player) { }

        public override void EnterState()
        {
            _cachedXSpeed = Mathf.Abs(Player.Velocity.x);
            _dashTimer = Stats.DashTime;

            // Dash goes in direction of input rather than player facing direction
            _dashDirection = (InputInfo.Move.x == 0)
                ? (Player.IsFacingRight ? 1 : -1)
                : (InputInfo.Move.x > 0 ? 1 : -1);

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
            GameObject effectObject;
            if (TriggerInfo.OnGround)
            {
                if (Player.SlideEffect != null)
                {
                    effectObject = GameObject.Instantiate(Player.SlideEffect);
                    effectObject.transform.parent = Player.gameObject.transform;
                }
            }
            else if (Player.DashEffect != null)
            {
                effectObject = GameObject.Instantiate(Player.DashEffect);
                effectObject.transform.parent = Player.gameObject.transform;
            }

            if (_isSlide)
            {
                Player.Animator.SetBool("Sliding", true);
            }
            else
                Player.Animator.SetBool("Dashing", true);

            Player.GiveInvincibility(200f); // TODO: Think of better implementation
        }

        public override void UpdateState()
        {
            if (_isSlide)
                DealDamage();
        }

        public override void FixedUpdateState()
        {
            if (_dashTimer < 0f)
                Player.SetState(PlayerStateType.Move);

            if (TriggerInfo.HitWallThisFrame && !TriggerInfo.OnGround)
                _hitWall = true;

            _dashTimer -= Time.fixedDeltaTime;
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

            Player.StopInvincibility();
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.SlideAttack);
            foreach (var enemy in enemies)
            {
                enemy.AirLaunch(Player.IsFacingRight);
            }
        }
    }
}
