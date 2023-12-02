using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class AirAttack1State : PlayerState
    {
        private float _attackTimer;
        private bool _attackPressedAgain;
        private bool _enemyHit;

        public AirAttack1State(Player player) : base(player) { }

        public override void EnterState()
        {
            _attackTimer = 0;
            _enemyHit = false;
            Player.Animator.SetTrigger("AirAttack1");
            Player.GroundAttack1Hitbox.enabled = true;
            Player.SetGravity(0f);
            _attackPressedAgain = false;
            Player.UseAttack();
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
            float curveSample = Stats.AirAttack1MovementCurve.Evaluate(1 - _attackTimer / Stats.GroundAttack1Length); // TODO: change to air attack length
            Player.SetVelocity((Player.IsFacingRight ? 1 : -1) * curveSample * Stats.AirAttack1MovementStrength, 0);
        }

        public override void ExitState()
        {
            Player.GroundAttack1Hitbox.enabled = false; // TODO: change to air attack
            Player.SetGravity(Stats.FallingGravity);
        }

        private void HandleStateChange()
        {
            if (_attackTimer > Stats.GroundAttack1Length)
            {
                if (_attackPressedAgain && _enemyHit)
                    Player.SetState(PlayerStateType.AirAttack2);
                else
                {
                    Player.SetGravity(Stats.RisingGravity);
                    Player.UseAttack();
                    Player.SetState(PlayerStateType.Move);
                }
            }
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(Player.GroundAttack1Hitbox);
            if (enemies.Count > 0) _enemyHit = true;
            foreach (var enemy in enemies)
            {
                // TODO: use air attack knockback stats
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

