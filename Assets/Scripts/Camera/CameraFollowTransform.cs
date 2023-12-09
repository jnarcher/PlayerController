using UnityEngine;

public class CameraFollowTransform : MonoBehaviour
{
    [SerializeField] private float _verticalLookDistance = 2f;
    [Space]
    [SerializeField] private InputInfo _ipt;
    [SerializeField] private TriggerInfo _trigs;
    [SerializeField] private PlayerStateMachine.Player _controller;
    [SerializeField] private Transform _playerTransform;

    private float _followX;
    private float _followY;

    private void Update()
    {
        HandleFollowX();
        HandleFollowY();

        transform.position = new Vector2(_followX, _followY);
        // transform.position = _playerTransform.position;
    }

    private void HandleFollowY()
    {
        _followY = _playerTransform.position.y;
        if (Mathf.Abs(_ipt.Camera) == 1 && _trigs.OnGround && Mathf.Abs(_controller.Velocity.x) < 0.001f)
            _followY += _ipt.Camera * _verticalLookDistance;
    }

    private void HandleFollowX()
    {
        _followX = _playerTransform.position.x;
        // _followX += _horizontalLookAheadDistance * (_controller.IsFacingRight ? 1 : -1);
    }
}
