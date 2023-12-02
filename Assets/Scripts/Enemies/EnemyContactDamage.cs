using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyContactDamage : MonoBehaviour
{
    public EnemyStats Stats;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 knockbackDir = (other.transform.position - transform.position).normalized;

            other.gameObject.GetComponent<PlayerHealth>().Damage(Stats.ContactDamage, Stats.ContactKnockbackStrength * knockbackDir);
        }
    }
}
