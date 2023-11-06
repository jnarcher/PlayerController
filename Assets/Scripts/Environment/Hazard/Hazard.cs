using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int HazardDamage = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            GameManager.Instance.DamageAndRespawn(HazardDamage);
    }
}
