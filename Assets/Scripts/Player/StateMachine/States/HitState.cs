using UnityEngine;

namespace PlayerStateMachine
{
    public class HitState : PlayerState
    {
        private float knockbackDirectionX;
        public HitState(Player player, PlayerStateType stateType) : base(player, stateType) { }

        public override void EnterState()
        {
            Player.GiveInvincibility(Stats.HitInvincibilityTime);
            Player.Animator.SetTrigger("Hit");
            knockbackDirectionX = Player.HitDirection.x > 0f ? 1f : -1f;

            if (Player.HitEffect != null)
                GameObject.Instantiate(Player.HitEffect, Player.Position, Quaternion.identity);
        }

        public override void FixedUpdateState()
        {
            Vector2 knockback = Stats.HitKnockbackStrength * Player.AnimatedVelocity;
            knockback.x *= knockbackDirectionX;
            Player.SetVelocity(knockback);
        }

        public override void UpdateState()
        {
            CheckTransitionState();
        }

        public override void ExitState()
        {
            Player.ResetAttack();
            Player.ResetDash();
            Player.ResetAirJumps();
        }

        private void CheckTransitionState()
        {
            if (Player.TryUseAnimationCompleteTrigger())
                Player.SetState(PlayerStateType.Move);
        }
    }
}
