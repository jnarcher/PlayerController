using UnityEngine;

public class BreakableWall : MonoBehaviour, IEnemyController
{
    public Vector2 DirectionHitFrom { get; set; }
    public float HitStrength { get; set; }

    public EnemyStats Stats;

    public void AirLaunch(bool toRight) { }
    public void Freeze() { }
    public void Stun() { }
    public void UnFreeze() { }

    public EnemyStats GetStats() => Stats;

    public void Kill()
    {
        Object.Destroy(gameObject);
    }

}
