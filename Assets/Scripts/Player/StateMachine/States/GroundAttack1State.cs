using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class GroundAttack1State : PlayerState
    {
        // private float _attackTimer;
        private bool _attackPressedAgain;
        private float _cachedXSpeed;

        public GroundAttack1State(Player player) : base(player) { }

        public override void EnterState()
        {
            // _attackTimer = 0;
            Player.Animator.SetTrigger("GroundAttack1");
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
            // _attackTimer += Time.deltaTime;
            DealDamage();
            CheckForComboInput();
            HandleStateChange();
        }

        public override void FixedUpdateState()
        {
            float xDirection = Player.IsFacingRight ? 1 : -1;
            float newXSpeed = (0.5f * _cachedXSpeed) + Player.AnimatedVelocity * Stats.GroundAttack1MovementStrength;
            Player.SetVelocity(xDirection * newXSpeed, 0);
        }

        private void HandleStateChange()
        {
            // if (_attackTimer > Stats.GroundAttack1Length)
            if (Player.AttackAnimationComplete)
            {
                Player.AttackAnimationComplete = false; // reset trigger

                if (_attackPressedAgain) // Start combo
                    Player.SetState(PlayerStateType.GroundAttack2);
                else // Combo ended
                {
                    Player.SetState(PlayerStateType.Move);
                    Player.UseAttack();
                    Player.SetGravity(GameManager.Instance.PlayerStats.RisingGravity);
                }
            }
        }

        private List<EnemyHealth> GetEnemiesInHitbox()
        {
            List<Collider2D> hits = new();
            ContactFilter2D filter = new()
            {
                useTriggers = true,
            };
            Physics2D.OverlapCollider(TriggerInfo.GroundAttack1, filter, hits);

            List<EnemyHealth> enemies = new();
            foreach (var hit in hits)
            {
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemies.Add(enemyHealth);
            }

            return enemies;
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.GroundAttack1);
            foreach (var enemy in enemies)
            {
                enemy.Damage(
                    Stats.GroundAttackDamage,
                    Stats.GroundAttack1KnockbackStrength * (Player.IsFacingRight ? 1 : -1) * Vector2.right
                );
            }
        }

        private void CheckForComboInput()
        {
            if (InputInfo.AttackPressedThisFrame)
                _attackPressedAgain = true;
        }
    }
}
