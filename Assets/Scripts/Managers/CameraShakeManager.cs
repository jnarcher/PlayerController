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
        SetRumblePulse(ControllerRumbleIntensity, ControllerRumbleDuration);
    }

    private float _currentStrength = 0;
    private float _rumbleDuration;
    public void SetRumblePulse(float strength, float duration)
    {
        pad = Gamepad.current;
        _currentStrength = strength;
        _rumbleDuration = duration;
        StartCoroutine(DoRumble());
    }

    private IEnumerator DoRumble()
    {
        float time = 0;
        while (time < _rumbleDuration)
        {
            float newStrength = Mathf.Lerp(_currentStrength, 0, time / _rumbleDuration);
            pad?.SetMotorSpeeds(newStrength, 0.7f * newStrength);
            time += Time.deltaTime;
            yield return null;
        }
        pad?.SetMotorSpeeds(0, 0);
    }
}
