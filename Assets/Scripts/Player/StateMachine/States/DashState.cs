using UnityEngine;

namespace PlayerStateMachine
{
    /// <summary>
    /// This handles the logic for a dash.
    /// </summary>
    public class DashState : PlayerState
    {
        private float _dashStartTime;
        private float _dashDirection;
        private float _cachedXVelocity;
        private float DashSpeed => Stats.DashDistance / Stats.DashTime;

        public DashState(Player player) : base(player) { }

        public override void EnterState()
        {
            _dashStartTime = Player.ElapsedTime;
            _cachedXVelocity = Player.Velocity.x;
            _dashDirection = Player.IsFacingRight ? 1 : -1;

            Player.SetVelocity(_dashDirection * DashSpeed, 0);
            Player.SetGravity(0);

            Player.SetAnimation("Dash");
        }

        public override void UpdateState() { }

        public override void FixedUpdateState()
        {
            if (Player.ElapsedTime >= _dashStartTime + Stats.DashTime)
                Player.SetState(PlayerStateType.Move);
        }

        public override void ExitState()
        {
            Player.SetGravity(Stats.RisingGravity);
            Player.SetDashCooldown();
        }
    }
}
