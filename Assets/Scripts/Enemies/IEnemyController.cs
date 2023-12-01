using UnityEngine;

public interface IEnemyController
{
    public void Knockback(Vector2 knockback);
    public void Kill();
    public EnemyStats GetStats();
}
