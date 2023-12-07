using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class UpAttackState : PlayerState
    {
        private List<EnemyHealth> _hitEnemies;

        public UpAttackState(Player player, PlayerStateType stateType) : base(player, stateType)
        {
            _hitEnemies = new();
        }

        public override void EnterState()
        {
            Player.Animator.SetTrigger("UpAttack");
            InputInfo.UseAttack();
        }

        public override void UpdateState()
        {
            DealDamage();
            CheckStateTransitions();
        }

        public override void ExitState()
        {
            ResetEnemyHitables();
            Player.UseAttack();
        }

        private void ResetEnemyHitables()
        {
            foreach (var enemy in _hitEnemies)
                enemy.HasTakenDamage = false;
            _hitEnemies.Clear();
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.UpAttack);
            foreach (var enemy in enemies)
            {
                if (!enemy.HasTakenDamage)
                {
                    _hitEnemies.Add(enemy);
                    enemy.Damage(
                        Stats.GroundAttackDamage,
                        Vector2.up,
                        Stats.UpAttackKnockbackStrength
                    );
                }
            }
        }

        private void CheckStateTransitions()
        {
            if (Player.TryUseAnimationCompleteTrigger())
                Player.SetState(PlayerStateType.Move);
        }
    }
}
