using UnityEngine;

namespace PlayerStateMachine
{
    public class GrappleLaunchState : PlayerState
    {
        public GrappleLaunchState(Player player) : base(player) { }

        public override void EnterState()
        {
            if (Player.SelectedGrapplePoint == null)
            {
                Player.SetState(PlayerStateType.Move);
                Debug.LogWarning("No grapple point selected for launch.");
                return;
            }
            Player.SetFacing(Player.transform.position.x < Player.SelectedGrapplePoint.transform.position.x);
            Player.GiveInvincibility(100f); // the time is arbitrary as long as it's greater than the time it takes to get to the grapple point
        }

        private Vector3 PointPosition => Player.SelectedGrapplePoint.transform.position;

        public override void FixedUpdateState()
        {
            if (Vector2.Distance(Player.transform.position, PointPosition) <= Stats.GrappleStopDistance)
            {
                Player.SetState(PlayerStateType.Move);
                return;
            }
            Vector2 grapplePointDirection = (PointPosition - Player.transform.position).normalized;
            Player.SetVelocity(Stats.GrappleSpeed * grapplePointDirection);
        }

        public override void ExitState()
        {
            Player.ResetAirJumps();
            Player.ResetDash();
            Player.ResetAttack();
            Player.LerpMoveAcceleration(Stats.GrappleInputFreezeTime);
            Player.SelectedGrapplePoint?.GetComponent<GrapplePointController>().StartCooldown();
            Player.GiveInvincibility(0.2f);
        }
    }
}
