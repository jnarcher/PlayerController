using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class DownAttackState : PlayerState
    {
        private List<EnemyHealth> _hitEnemies;
        private bool _hitEnemy;

        public DownAttackState(Player player, PlayerStateType stateType) : base(player, stateType)
        {
            _hitEnemies = new();
        }

        public override void EnterState()
        {
            Player.Animator.SetTrigger("DownAttack");
            _hitEnemy = false;
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
            if (!_hitEnemy) Player.UseAttack();
            else Player.ResetAttack();
        }

        private void ResetEnemyHitables()
        {
            foreach (var enemy in _hitEnemies)
                enemy.HasTakenDamage = false;
            _hitEnemies.Clear();
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.DownAttack);

            if (!_hitEnemy && enemies.Count > 0)
            {
                Pogo();
                _hitEnemy = true;
            }

            foreach (var enemy in enemies)
            {
                if (!enemy.HasTakenDamage)
                {
                    _hitEnemies.Add(enemy);
                    enemy.Damage(
                        Stats.AirAttackDamage,
                        Vector2.down,
                        Stats.DownAttackKnockbackStrength
                    );
                }
            }
        }

        private void CheckStateTransitions()
        {
            if (Player.TryUseAnimationCompleteTrigger())
                Player.SetState(PlayerStateType.Move);
        }

        private void Pogo()
        {
            Player.SetVelocity(Player.Velocity.x, Stats.DownAttackPogoStrength);
            Player.ResetDash();
            Player.ResetAirJumps();
        }
    }
}