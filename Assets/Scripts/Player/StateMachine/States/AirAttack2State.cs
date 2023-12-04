using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class AirAttack2State : PlayerState
    {
        private float _attackTimer;
        private float _cachedXSpeed;

        public AirAttack2State(Player player) : base(player) { }

        public override void EnterState()
        {
            _attackTimer = 0;
            _cachedXSpeed = Player.Velocity.x;
            Player.Animator.SetTrigger("AirAttack2");
            Player.SetGravity(0f);
            Player.UseAttack();
        }

        public override void UpdateState()
        {
            _attackTimer += Time.deltaTime;
            DealDamage();
            HandleStateChange();
        }

        public override void FixedUpdateState()
        {
            Player.SetVelocity(0.5f * _cachedXSpeed, 0);
        }

        public override void ExitState()
        {
            Player.SetGravity(Stats.FallingGravity);
        }

        private void HandleStateChange()
        {
            if (Player.AttackAnimationComplete)
            {
                Player.AttackAnimationComplete = false;
                Player.SetGravity(Stats.RisingGravity);
                Player.UseAttack();
                Player.SetState(PlayerStateType.Move);
            }
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.AirAttack2);
            foreach (var enemy in enemies)
            {
                // TODO: use air attack knockback stats
                enemy.Damage(
                    Stats.GroundAttackDamage,
                    Stats.GroundAttack1KnockbackStrength * (Player.IsFacingRight ? 1 : -1) * Vector2.right
                );
            }
        }
    }
}
