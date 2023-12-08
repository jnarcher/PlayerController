using UnityEngine;

public class CameraFollowTransform : MonoBehaviour
{
    [SerializeField] private float LookDistance = 2f;

    private InputInfo _ipt;
    private TriggerInfo _trigs;
    private PlayerStateMachine.Player _controller;

    private void Awake()
    {
        _ipt = GetComponentInParent<InputInfo>();
        _trigs = GetComponentInParent<TriggerInfo>();
        _controller = GetComponentInParent<PlayerStateMachine.Player>();
    }

    private void Update()
    {
        if (_trigs.OnGround && Mathf.Abs(_controller.Velocity.x) < 0.001f)
            transform.position = _ipt.Camera * LookDistance * Vector2.up + _controller.Position;
        else
            transform.position = _controller.Position;
    }

}
