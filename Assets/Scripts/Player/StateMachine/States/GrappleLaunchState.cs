using UnityEngine;

namespace PlayerStateMachine
{
    public class GrappleLaunchState : PlayerState
    {
        public GrappleLaunchState(Player player, PlayerStateType stateType) : base(player, stateType) { }

        private Vector2 _gpDirection;

        public override void EnterState()
        {
            if (Player.SelectedGrapplePoint == null)
            {
                Player.SetState(PlayerStateType.Move);
                Debug.LogWarning("No grapple point selected for launch.");
                return;
            }
            Player.SetFacing(Player.transform.position.x < Player.SelectedGrapplePoint.transform.position.x); Player.GiveInvincibility(100f); // the time is arbitrary as long as it's greater than the time it takes to get to the grapple point
        }

        private Vector3 PointPosition => Player.SelectedGrapplePoint.transform.position;

        public override void FixedUpdateState()
        {
            if (Vector2.Distance(Player.transform.position, PointPosition) <= Stats.GrappleStopDistance)
            {
                Player.SetState(PlayerStateType.Move);
                return;
            }

            _gpDirection = (PointPosition - Player.transform.position).normalized;
            Player.SetVelocity(Stats.GrappleSpeed * _gpDirection);
        }

        public override void ExitState()
        {
            Player.ResetAirJumps();
            Player.ResetDash();
            Player.ResetAttack();
            Player.LerpMoveAcceleration(Stats.GrappleInputFreezeTime);
            Player.GiveInvincibility(0.2f);
            Player.SetVelocity(Stats.GrappleLaunchBoostMultiplier * Stats.GrappleSpeed * _gpDirection);
            ResetGrapple();
        }

        private void ResetGrapple()
        {
            GrapplePointController gpController = Player.SelectedGrapplePoint?.GetComponent<GrapplePointController>();
            gpController.StartCooldown();
            gpController.Grappleable?.UnFreeze();
            Player.ResetSelectedGrapplePoint();
        }
    }
}
