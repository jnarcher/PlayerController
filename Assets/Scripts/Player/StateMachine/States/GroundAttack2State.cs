using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class GroundAttack2State : PlayerState
    {
        private float _attackTimer;

        public GroundAttack2State(Player player) : base(player) { }

        public override void EnterState()
        {
            _attackTimer = 0;
            Player.Animator.SetTrigger("GroundAttack2");
            Player.GroundAttack1Hitbox.enabled = true;
        }

        public override void UpdateState()
        {
            _attackTimer += Time.deltaTime;
            DealDamage();
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
                Player.SetState(PlayerStateType.Move);
                Player.UseAttack();
                Player.SetGravity(GameManager.Instance.PlayerStats.RisingGravity);
            }
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(Player.GroundAttack1Hitbox);
            foreach (var enemy in enemies)
            {
                enemy.Damage(
                    Stats.GroundAttackDamage,
                    Stats.GroundAttack2KnockbackStrength * (Player.IsFacingRight ? 1 : -1) * Vector2.right
                );
            }
        }
    }
}