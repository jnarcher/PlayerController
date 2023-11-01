using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CameraSettings _settings;

    private CinemachineVirtualCamera _vCam;

    private float _shakeTimer;
    private float _shakeLength;
    private float _intensity;

    private void Awake()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cbmcp = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cbmcp.m_AmplitudeGain = Mathf.Lerp(0f, _intensity, _shakeTimer / _shakeLength);
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        if (!_settings.ToggleShake) return;

        CinemachineBasicMultiChannelPerlin cbmcp = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cbmcp.m_AmplitudeGain = intensity;
        _intensity = intensity;
        _shakeLength = time;
        _shakeTimer = time;
    }
}
