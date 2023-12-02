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
    public int MaxHealth = 2;

    [Header("Air Launched State")]
    public float AirLaunchedStateLength = 0.5f;
    public float AirLaunchCurveStrengthX = 40f;
    public AnimationCurve AirLaunchVelocityX;
    public float AirLaunchCurveStrengthY = 40f;
    public AnimationCurve AirLaunchVelocityY;

}
