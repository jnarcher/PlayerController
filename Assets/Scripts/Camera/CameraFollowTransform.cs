using UnityEngine;

public class CameraFollowTransform : MonoBehaviour
{
    [SerializeField] private float _verticalLookDistance = 2f;
    [SerializeField] private float _horizontalLookAheadDistance = 1f;
    [SerializeField] private float _lookAheadLerpTime = 1f;
    [Space]
    [SerializeField] private InputInfo _ipt;
    [SerializeField] private TriggerInfo _trigs;
    [SerializeField] private PlayerStateMachine.Player _controller;

    private float followX;
    private float followY;

    private void Update()
    {
        HandleFollowX();
        HandleFollowY();

        transform.position = new Vector2(followX, followY);
    }

    private void HandleFollowY()
    {
        followY = _controller.Position.y;
        if (Mathf.Abs(_ipt.Camera) == 1 && _trigs.OnGround && Mathf.Abs(_controller.Velocity.x) < 0.001f)
            followY += _ipt.Camera * _verticalLookDistance;
    }

    private void HandleFollowX()
    {
        followX = _controller.Position.x;
        followX += _horizontalLookAheadDistance * (_controller.IsFacingRight ? 1 : -1);
    }
}
