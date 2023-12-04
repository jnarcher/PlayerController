using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public virtual void Damage(int damage, Vector2 direction, float knockbackStrength) { }
    public virtual void Kill() { }

}
