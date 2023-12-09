using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    [Range(0f, 1f)] public float ControllerRumbleIntensity = 1f;
    [Range(0f, 1f)] public float ControllerRumbleDuration = 0.2f;

    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;

        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void CameraShake(float force)
    {
        _impulseSource.GenerateImpulseWithForce(force);
    }
}
