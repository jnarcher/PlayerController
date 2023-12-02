using UnityEngine;

public interface IEnemyController
{
    public void Knockback(Vector2 knockback);
    public void Kill();
    public void AirLaunch(bool toRight);
    public EnemyStats GetStats();
}
