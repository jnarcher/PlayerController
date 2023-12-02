using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class GroundAttack1State : PlayerState
    {
        private float _attackTimer;
        private bool _attackPressedAgain;

        public GroundAttack1State(Player player) : base(player) { }

        public override void EnterState()
        {
            _attackTimer = 0;
            Player.Animator.SetTrigger("GroundAttack1");
            Player.GroundAttack1Hitbox.enabled = true;
            _attackPressedAgain = false;
        }

        public override void UpdateState()
        {
            _attackTimer += Time.deltaTime;
            DealDamage();
            CheckForComboInput();
            HandleStateChange();
        }

        public override void FixedUpdateState()
        {
            float curveSample = Stats.GroundAttack1MovementCurve.Evaluate(1 - _attackTimer / Stats.GroundAttack1Length);
            Player.SetVelocity((Player.IsFacingRight ? 1 : -1) * curveSample * Stats.GroundAttack1MovementStrength, 0);
        }

        public override void ExitState()
        {
            Player.GroundAttack1Hitbox.enabled = false;
        }

        private void HandleStateChange()
        {
            if (_attackTimer > Stats.GroundAttack1Length)
            {
                if (_attackPressedAgain) // Start combo
                {
                    Player.SetState(PlayerStateType.GroundAttack2);
                }
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
            Physics2D.OverlapCollider(Player.GroundAttack1Hitbox, filter, hits);

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
            List<EnemyHealth> enemies = GetEnemiesInHitbox();
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