using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumbleManager : MonoBehaviour
{
    public static ControllerRumbleManager Instance;

    [SerializeField] private AnimationCurve _lowFrequencyCurve;
    [SerializeField] private AnimationCurve _highFrequencyCurve;

    private Gamepad pad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    private float _rumbleStrength;
    private float _rumbleDuration;
    public void SetRumblePulse(float strength, float duration)
    {
        if (_inRumblePulse) return;

        pad = Gamepad.current;
        _rumbleStrength = Mathf.Clamp01(strength);
        _rumbleDuration = duration;
        StartCoroutine(DoRumblePulse());
    }

    private bool _inRumblePulse;
    private IEnumerator DoRumblePulse()
    {
        _inRumblePulse = true;
        float time = 0;
        while (time < _rumbleDuration)
        {
            float t = time / _rumbleDuration;
            float lowFreq = _rumbleStrength * _lowFrequencyCurve.Evaluate(t);
            float highFreq = _rumbleStrength * _highFrequencyCurve.Evaluate(t);
            pad?.SetMotorSpeeds(lowFreq, highFreq);
            time += Time.deltaTime;
            yield return null;
        }
        pad?.SetMotorSpeeds(0, 0);
        _inRumblePulse = false;
    }
}
