using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public float Acceleration = 200f;
    public float Speed = 10f;
    public float Gravity = 200f;

    public int ContactDamage = 1;
    public float KnockbackStrength = 30f;
}
