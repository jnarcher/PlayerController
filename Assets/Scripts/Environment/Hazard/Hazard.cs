using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hazard : MonoBehaviour
{
    public int HazardDamage = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.DamageAndRespawn(HazardDamage, GetDirection()); // fix knockback direction
            GameManager.Instance.RespawnPlayer();
        }
        else
            other.gameObject.GetComponent<EnemyHealth>()?.Kill();
    }

    private Vector2 GetDirection()
    {
        // TODO: how to calculate the facing direction of the hazard
        return Vector2.right;
    }
}
