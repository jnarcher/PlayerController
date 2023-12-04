using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class AirAttack1State : PlayerState
    {
        private bool _attackPressedAgain;
        private bool _enemyHit;
        private float _cachedXVelocity;

        public AirAttack1State(Player player) : base(player) { }

        public override void EnterState()
        {
            _enemyHit = false;
            _attackPressedAgain = false;
            _cachedXVelocity = Player.Velocity.x;
            Player.Animator.SetTrigger("AirAttack1");
            Player.SetGravity(0f);
            Player.UseAttack();

            // Allow quick turn attacks
            if (InputInfo.Move.x != 0 && InputInfo.Move.x > 0 != Player.IsFacingRight)
                Player.SetFacing(InputInfo.Move.x > 0);
        }

        public override void UpdateState()
        {
            DealDamage();
            CheckForComboInput();
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

                if (_enemyHit)
                {
                    Player.ResetDash();
                    Player.ResetAirJumps();

                    if (_attackPressedAgain)
                        Player.SetState(PlayerStateType.AirAttack2);
                }
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
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(TriggerInfo.AirAttack1);
            if (enemies.Count > 0) _enemyHit = true;
            foreach (var enemy in enemies)
            {
                enemy.Damage(
                    Stats.AirAttackDamage,
                    Stats.AirAttack1KnockbackStrength * (Player.IsFacingRight ? 1 : -1) * Vector2.right
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

