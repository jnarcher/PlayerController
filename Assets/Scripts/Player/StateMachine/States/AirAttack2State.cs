using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class AirAttack2State : PlayerState
    {
        private float _attackTimer;
        private float _cachedXVelocity;

        public AirAttack2State(Player player) : base(player) { }

        public override void EnterState()
        {
            _attackTimer = 0;
            _cachedXVelocity = Player.Velocity.x;
            Player.Animator.SetTrigger("AirAttack2");
            Player.SetGravity(0f);
            Player.UseAttack();

            // Allow quick turn attacks
            if (InputInfo.Move.x != 0 && InputInfo.Move.x > 0 != Player.IsFacingRight)
                Player.SetFacing(InputInfo.Move.x > 0);
        }

        public override void UpdateState()
        {
            _attackTimer += Time.deltaTime;
            DealDamage();
            HandleStateChange();
        }

        public override void FixedUpdateState()
        {
            Player.SetVelocity(0.5f * _cachedXVelocity, 0);
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
                enemy.Damage(
                    Stats.AirAttackDamage,
                    Stats.AirAttack2KnockbackStrength * (Player.IsFacingRight ? 1 : -1) * Vector2.right
                );
            }
        }
    }
}
