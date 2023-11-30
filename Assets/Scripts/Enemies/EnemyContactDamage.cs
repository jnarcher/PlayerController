using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyContactDamage : MonoBehaviour
{
    public int Damage;
    public float KnockbackStrength;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
            other.gameObject.GetComponent<PlayerHealth>().Damage(Damage, KnockbackStrength * knockbackDir);
        }
    }
}
