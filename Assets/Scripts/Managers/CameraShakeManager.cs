using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

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
    }
}
