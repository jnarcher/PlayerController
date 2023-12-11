using UnityEngine;

namespace PlayerStateMachine
{
    public class GrappleAimState : PlayerState
    {
        public GrappleAimState(Player player, PlayerStateType stateType) : base(player, stateType) { }

        private bool _launched;

        private float _aimTimer;

        public override void EnterState()
        {
            SoundManager.Instance.PlaySound(Player.Sounds.GrappleAim);
            Player.GrappleAimIndicator.SetActive(true);
            GameManager.Instance.LerpTimeScale(Stats.GrappleTimeSlow, Stats.GrappleTimeSlowTransitionSpeed);
            _launched = false;
            _aimTimer = 0;
        }

        public override void UpdateState()
        {
            _aimTimer += Time.unscaledDeltaTime;
            GameObject selectedGrapplePoint = FindGrappleFromInput();
            if (selectedGrapplePoint != Player.SelectedGrapplePoint)
            {
                Player.SetSelectedGrapplePoint(selectedGrapplePoint);
                if (selectedGrapplePoint != null) // small controller rumble everytime a grapple point is selected
                    ControllerRumbleManager.Instance.SetRumblePulse(0.2f, 0.2f);
            }

            if (!InputInfo.Grapple || _aimTimer > Stats.GrappleAimTime)
            {
                if (_aimTimer > Stats.GrappleAimTime)
                {
                    Player.SetGrappleCooldown();
                    selectedGrapplePoint = null;
                    CameraShakeManager.Instance.CameraShake(0.8f);
                    ControllerRumbleManager.Instance.SetRumblePulse(0.3f, 0.3f);
                }

                if (selectedGrapplePoint == null)
                    Player.SetState(PlayerStateType.Move);
                else
                {
                    _launched = true;
                    Player.SetState(PlayerStateType.GrappleLaunch);
                    selectedGrapplePoint.GetComponent<GrapplePointController>().Grappleable?.Freeze();
                }
            }
        }

        public override void FixedUpdateState()
        {
            Player.SetGravity(TriggerInfo.OnGround ? Stats.GroundingForce : (Player.Velocity.y >= 0 ? Stats.RisingGravity : Stats.FallingGravity));
        }

        private GameObject FindGrappleFromInput()
        {
            if (InputInfo.Aim == Vector2.zero) return null;

            GameObject chosenGrapplePoint = null;
            float grapplePointDist = float.MaxValue;
            foreach (var point in Player.ActiveGrapplePoints)
            {
                if (point == null) continue; // TODO: This solves the issue where an enemy is destroyed however they don't clear the grapple point from the active grapple points list. Find a better solution?

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
            if (!_launched)
                GameManager.Instance.LerpTimeScale(1, Stats.GrappleTimeSlowTransitionSpeed);
            else
                GameManager.Instance.ResetTimeScale();
            Player.GrappleAimIndicator.SetActive(false);
        }
    }
}
