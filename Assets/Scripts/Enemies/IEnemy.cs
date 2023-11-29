using UnityEngine;

public interface IEnemy
{
    public void Damage(int amount, Vector2 knockback);
    public void Kill();
}
