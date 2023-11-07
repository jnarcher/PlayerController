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
        private float _cachedXSpeed;
        private float DashSpeed => Stats.DashDistance / Stats.DashTime;

        public DashState(Player player) : base(player) { }

        public override void EnterState()
        {
            _dashStartTime = Player.ElapsedTime;
            _cachedXSpeed = Mathf.Abs(Player.Velocity.x);
            _dashDirection = Player.IsFacingRight ? 1 : -1;

            // if on wall, dash away from the wall
            if (TriggerInfo.OnWall)
            {
                _dashDirection *= -1;
                _cachedXSpeed = DashSpeed;
                Player.SetFacing(!Player.IsFacingRight);
            }

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
            Player.SetVelocity(_dashDirection * _cachedXSpeed, 0);
            Player.SetDashCooldown();
        }
    }
}
