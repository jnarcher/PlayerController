using UnityEngine;

public interface IEnemyController
{
    public void Kill();
    public void AirLaunch(bool toRight);
    public EnemyStats GetStats();

    public Vector2 DirectionHitFrom { get; set; }
    public float HitStrength { get; set; }

    public void Stun();
    public void Freeze();
    public void UnFreeze();
}
