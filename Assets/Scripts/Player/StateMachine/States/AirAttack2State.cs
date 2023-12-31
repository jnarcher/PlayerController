using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class AirAttack2State : PlayerState
    {
        private float _attackTimer;
        private float _cachedXVelocity;

        private List<EnemyHealth> _hitEnemies;

        public AirAttack2State(Player player, PlayerStateType stateType) : base(player, stateType)
        {
            _hitEnemies = new();
        }

        public override void EnterState()
        {
            _attackTimer = 0;
            _cachedXVelocity = Player.Velocity.x;
            Player.Animator.SetTrigger("AirAttack2");
            Player.SetGravity(0f);
            Player.SetAttackCooldown();
            InputInfo.UseAttack();

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
            Player.SetVelocity(0.7f * _cachedXVelocity, 0);
        }

        public override void ExitState()
        {
            ResetEnemyHitables();
            Player.SetGravity(Stats.FallingGravity);
            Player.SetAttackCooldown();
        }

        private void ResetEnemyHitables()
        {
            foreach (var enemy in _hitEnemies)
                enemy.HasTakenDamage = false;
            _hitEnemies.Clear();
        }

        private void HandleStateChange()
        {
            if (Player.TryUseAnimationCompleteTrigger())
                Player.SetState(PlayerStateType.Move);
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.AirAttack2);
            foreach (var enemy in enemies)
            {
                if (!enemy.HasTakenDamage)
                {
                    _hitEnemies.Add(enemy);
                    enemy.Damage(
                        Stats.AirAttackDamage,
                        (Player.IsFacingRight ? 1 : -1) * Vector2.right,
                        Stats.AirAttack2KnockbackStrength
                    );
                }
            }
        }
    }
}
