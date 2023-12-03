using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement")]
    public float Acceleration = 200f;
    public float Speed = 10f;
    public float Gravity = 200f;

    [Header("Player Contact")]
    public int ContactDamage = 1;
    public float ContactKnockbackStrength = 30f;

    [Header("Health")]
    public bool Damageable = true;
    public int MaxHealth = 2;
    public float StunTime = 0.2f;

    [Header("Air Launched State")]
    public float AirLaunchedStateLength = 0.5f;
    [Space]
    public float AirLaunchCurveStrengthX = 40f;
    public AnimationCurve AirLaunchVelocityX;
    [Space]
    public float AirLaunchCurveStrengthY = 40f;
    public AnimationCurve AirLaunchVelocityY;
}
