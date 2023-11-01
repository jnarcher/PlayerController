using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraFollowController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraSettings _settings;

    [SerializeField] private CinemachineBrain _brain;
    private CinemachineVirtualCamera _activeCam;
    private CinemachineFramingTransposer _framingTransposer;


    private bool _isFacingRight;
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void Update() { }
}
