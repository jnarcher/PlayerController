using UnityEngine;

[CreateAssetMenu(menuName = "Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("SHAKE")]
    public bool ToggleShake = true;
    public float ShakeIntensity = 1f;
    public float ShakeTime = 0.21f;
}