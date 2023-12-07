using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class GroundAttack1State : PlayerState
    {
        private bool _attackPressedAgain;
        private float _cachedXSpeed;

        private List<EnemyHealth> _hitEnemies;

        public GroundAttack1State(Player player) : base(player)
        {
            _hitEnemies = new();
        }

        public override void EnterState()
        {
            Player.Animator.SetTrigger("GroundAttack1");
            InputInfo.UseAttack();
            _attackPressedAgain = false;
            _cachedXSpeed = Mathf.Abs(Player.Velocity.x);

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
            CheckForComboInput();
            HandleStateChange();
        }

        public override void FixedUpdateState()
        {
            float xDirection = Player.IsFacingRight ? 1 : -1;
            float newXSpeed = (0.5f * _cachedXSpeed) + Player.AnimatedVelocity.x * Stats.GroundAttack1MovementStrength;
            Player.SetVelocity(xDirection * newXSpeed, 0);
        }

        public override void ExitState()
        {
            ResetEnemyHitables();
            Player.UseAttack();
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
                Player.SetState(_attackPressedAgain
                    ? PlayerStateType.GroundAttack2
                    : PlayerStateType.Move
                );
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.GroundAttack1);
            foreach (var enemy in enemies)
            {
                if (!enemy.HasTakenDamage)
                {
                    _hitEnemies.Add(enemy);
                    enemy.Damage(
                        Stats.GroundAttackDamage,
                        (Player.IsFacingRight ? 1 : -1) * Vector2.right,
                        Stats.GroundAttack1KnockbackStrength
                    );
                }
            }
        }

        private void CheckForComboInput()
        {
            if (InputInfo.AttackPressedThisFrame)
                _attackPressedAgain = true;
        }
    }
}
