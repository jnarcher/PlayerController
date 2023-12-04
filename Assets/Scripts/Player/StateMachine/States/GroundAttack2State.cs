using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class GroundAttack2State : PlayerState
    {
        private float _cachedXSpeed;

        public GroundAttack2State(Player player) : base(player) { }

        public override void EnterState()
        {
            _cachedXSpeed = Player.Velocity.x;
            Player.Animator.SetTrigger("GroundAttack2");
        }

        public override void UpdateState()
        {
            DealDamage();
            HandleStateChange();
        }

        public override void FixedUpdateState()
        {
            float xDirection = Player.IsFacingRight ? 1 : -1;
            float newXVelocity = xDirection * Player.AnimatedVelocity * Stats.GroundAttack2MovementStrength;
            Player.SetVelocity(newXVelocity + (0.5f * _cachedXSpeed), 0);
        }

        public override void ExitState()
        {
        }

        private void HandleStateChange()
        {
            if (Player.AttackAnimationComplete)
            {
                Player.AttackAnimationComplete = false; // reset trigger
                Player.SetState(PlayerStateType.Move);
                Player.UseAttack();
                Player.SetGravity(GameManager.Instance.PlayerStats.RisingGravity);
            }
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.GroundAttack1);
            foreach (var enemy in enemies)
            {
                enemy.Damage(
                    Stats.GroundAttackDamage,
                    Stats.GroundAttack2KnockbackStrength * (Player.IsFacingRight ? 1 : -1) * Vector2.right
                );
            }
        }
    }
}