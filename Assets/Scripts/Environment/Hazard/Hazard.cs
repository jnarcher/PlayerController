using PlayerStateMachine;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hazard : MonoBehaviour
{
    public int HazardDamage = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if player health
        if (other.CompareTag("Player"))
        {
            Health health = other.gameObject.GetComponent<PlayerHealth>();
            if (health != null)
                GameManager.Instance.DamageAndRespawn(HazardDamage);
        }
        else
        {
            other.gameObject.GetComponent<EnemyHealth>()?.Kill();
        }
    }
}
