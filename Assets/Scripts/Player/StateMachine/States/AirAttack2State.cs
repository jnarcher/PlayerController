using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class AirAttack2State : PlayerState
    {
        private float _attackTimer;

        public AirAttack2State(Player player) : base(player) { }

        public override void EnterState()
        {
            _attackTimer = 0;
            Player.Animator.SetTrigger("AirAttack2");
            Player.GroundAttack1Hitbox.enabled = true; // TODO: use attack specific hitbox
            Player.SetGravity(0f);
            Player.UseAttack();
        }

        public override void UpdateState()
        {
            _attackTimer += Time.deltaTime;
            DealDamage();
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
                Player.SetGravity(Stats.RisingGravity);
                Player.UseAttack();
                Player.SetState(PlayerStateType.Move);
            }
        }

        private void DealDamage()
        {
            List<EnemyHealth> enemies = TriggerInfo.GetEnemiesInHitbox(Player.GroundAttack1Hitbox);
            foreach (var enemy in enemies)
            {
                // TODO: use air attack knockback stats
                enemy.Damage(
                    Stats.GroundAttackDamage,
                    Stats.GroundAttack1KnockbackStrength * (Player.IsFacingRight ? 1 : -1) * Vector2.right
                );
            }
        }
    }
}
