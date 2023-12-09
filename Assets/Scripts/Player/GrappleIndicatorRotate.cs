using PlayerStateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleIndicatorRotate : MonoBehaviour
{
    public float SmoothTime = 0.2f;

    private Player Player;
    private SpriteRenderer _sprite;
    private InputInfo InputInfo;
    private float _angle;

    private void Awake()
    {
        Player = GetComponentInParent<Player>();
        InputInfo = GetComponentInParent<InputInfo>();
        _sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        _sprite.enabled = false;
    }

    private void Update()
    {
        transform.position = Player.transform.position;

        if (InputInfo.Aim == Vector2.zero)
        {
            _sprite.enabled = false;
            return;
        }
        _sprite.enabled = true;


        float targetAngle;
        if (Player.SelectedGrapplePoint == null)
        {
            targetAngle = Mathf.Rad2Deg * Mathf.Atan2(InputInfo.Aim.y, InputInfo.Aim.x);
        }
        else
        {
            Vector2 dir = Player.SelectedGrapplePoint.transform.position - Player.transform.position;
            targetAngle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        }

        _angle = GetSmoothedAngle(targetAngle);

        transform.localEulerAngles = new Vector3(
            transform.rotation.x,
            Player.IsFacingRight ? 0 : 180, // correct for player turning
            _angle
        );
    }

    private float GetSmoothedAngle(float targetAngle)
    {
        // TODO: figure out how to smooth around a circle dealing with the discontinuity at 180 and -180
        return targetAngle;
    }
}
