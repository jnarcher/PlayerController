using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public virtual void Damage(int damage) { }
    public virtual void Kill() { }

}
