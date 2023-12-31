using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class GroundAttack2State : PlayerState
    {
        private float _cachedXSpeed;

        private List<EnemyHealth> _hitEnemies;

        public GroundAttack2State(Player player, PlayerStateType stateType) : base(player, stateType)
        {
            _hitEnemies = new();
        }

        public override void EnterState()
        {
            _cachedXSpeed = Mathf.Abs(Player.Velocity.x);
            InputInfo.UseAttack();
            Player.Animator.SetTrigger("GroundAttack2");

            // Allows quick turn attacks
            if (InputInfo.Move.x != 0 && InputInfo.Move.x > 0 != Player.IsFacingRight)
            {
                Player.SetFacing(InputInfo.Move.x > 0);
                _cachedXSpeed = 0f;
            }
        }

        public override void UpdateState()
        {
            DealDamage();
            HandleStateChange();
        }

        public override void FixedUpdateState()
        {
            float xDirection = Player.IsFacingRight ? 1 : -1;
            float newXSpeed = (0.5f * _cachedXSpeed) + Player.AnimatedVelocity.x * Stats.GroundAttack2MovementStrength;
            Player.SetVelocity(xDirection * newXSpeed, 0);
        }

        public override void ExitState()
        {
            ResetEnemyHitables();
            Player.SetAttackCooldown();
            Player.SetGravity(GameManager.Instance.PlayerStats.RisingGravity);
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
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.GroundAttack2);
            foreach (var enemy in enemies)
            {
                if (!enemy.HasTakenDamage)
                {
                    _hitEnemies.Add(enemy);
                    enemy.Damage(
                        Stats.GroundAttackDamage,
                        (Player.IsFacingRight ? 1 : -1) * Vector2.right,
                        Stats.GroundAttack2KnockbackStrength
                    );
                }
            }
        }
    }
}