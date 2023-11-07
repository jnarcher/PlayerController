using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class GrappleAimState : PlayerState
    {
        public GrappleAimState(Player player) : base(player) { }

        private float _timeStartedAiming;
        public override void EnterState()
        {
            Player.GrappleAimIndicator.SetActive(true);
            _timeStartedAiming = Player.ElapsedTime;
        }

        public override void UpdateState()
        {
            Time.timeScale = Mathf.Lerp(1, Stats.GrappleTimeSlow, (Player.ElapsedTime - _timeStartedAiming) / Stats.GrappleTimeSlowTransitionSpeed);

            GameObject selectedGrapplePoint = FindGrappleFromInput();
            Player.SetSelectedGrapplePoint(selectedGrapplePoint);

            if (!InputInfo.Grapple)
                Player.SetState(selectedGrapplePoint == null ? PlayerStateType.Move : PlayerStateType.GrappleLaunch);
        }

        private GameObject FindGrappleFromInput()
        {
            if (InputInfo.Aim == Vector2.zero) return null;

            GameObject chosenGrapplePoint = null;
            float grapplePointDist = float.MaxValue;
            foreach (var point in Player.ActiveGrapplePoints)
            {
                // get angle of grapple point from player
                Vector2 pointDirection = point.transform.position - Player.transform.position;
                float grapplePointAngle = Mathf.Atan2(pointDirection.y, pointDirection.x);

                // get angle of aim input
                float aimAngle = Mathf.Atan2(InputInfo.Aim.y, InputInfo.Aim.x);

                // Lerp the AimAssistAngle based on how close the grapple point is
                // The closer the point the larger angle
                float pointDistance = pointDirection.SqrMagnitude();
                float assistAngle = Mathf.Lerp(Stats.GrappleAssistAngle, 70, 1 - (pointDistance / (Stats.GrappleRange * Stats.GrappleRange)));

                float difference = Mathf.Rad2Deg * Mathf.Abs(aimAngle - grapplePointAngle);
                if (difference <= assistAngle && pointDistance < grapplePointDist)
                {
                    chosenGrapplePoint = point;
                    grapplePointDist = pointDistance;
                }
            }
            return chosenGrapplePoint;
        }

        public override void ExitState()
        {
            Time.timeScale = 1;
            Player.GrappleAimIndicator.SetActive(false);
        }
    }
}
