using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    private Gamepad pad;
    [Range(0f, 1f)] public float ControllerRumbleIntensity = 1f;
    [Range(0f, 1f)] public float ControllerRumbleDuration = 0.2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    public void CameraShake(CinemachineImpulseSource impulseSource, float force)
    {
        impulseSource.GenerateImpulseWithForce(force);
        ControllerRumbleManager.Instance.SetRumblePulse(1, 0.1f);
    }
}
