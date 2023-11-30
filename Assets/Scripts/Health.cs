using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public virtual void Damage(int damage, Vector2 knockback) { }
    public virtual void Kill() { }

}
