using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    public class GrappleAimState : PlayerState
    {
        private bool _failedExit;

        public GrappleAimState(Player player) : base(player) { }

        private float _timeStartedAiming;
        public override void EnterState()
        {
            Player.GrappleAimIndicator.SetActive(true);
            _timeStartedAiming = Player.ElapsedTime;
        }

        public override void UpdateState()
        {
            if (!_failedExit)
            {
                Time.timeScale = Mathf.Lerp(1, Stats.GrappleTimeSlow, (Player.ElapsedTime - _timeStartedAiming) / Stats.GrappleTimeSlowSpeed);

                GameObject selectedGrapplePoint = FindGrappleFromInput();
                Player.SetSelectedGrapplePoint(selectedGrapplePoint);

                if (!InputInfo.Grapple)
                {
                    if (selectedGrapplePoint == null)
                    {
                        _failedExit = true;
                        return;
                    }
                    Player.SetState(PlayerStateType.GrappleLaunch);
                }
            }
            else
            {
                Player.SetState(PlayerStateType.Move);
            }
        }

        private GameObject FindGrappleFromInput()
        {
            GameObject chosenGrapplePoint = null;
            float grapplePointDist = float.MaxValue;
            foreach (var point in Player.ActiveGrapplePoints)
            {
                // get angle of grapple point from player
                Vector2 pointDirection = point.transform.position - Player.transform.position;
                float grapplePointAngle = Mathf.Atan2(pointDirection.y, pointDirection.x);

                // get angle of aim input
                float aimAngle = Mathf.Atan2(InputInfo.Aim.y, InputInfo.Aim.x);

                // get distance between player and grapple point
                float pointDistance = pointDirection.SqrMagnitude();

                float difference = Mathf.Rad2Deg * Mathf.Abs(aimAngle - grapplePointAngle);
                if (difference <= Stats.GrappleAssistAngle && pointDistance < grapplePointDist)
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
            _failedExit = false;
            Player.GrappleAimIndicator.SetActive(false);
        }
    }
}
