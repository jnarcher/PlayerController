using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerWeapon : MonoBehaviour
{
    private Collider2D _hitbox;

    public void TurnOnHitbox() => _hitbox.enabled = true;
    public void TurnOffHitbox() => _hitbox.enabled = false;

    private void Awake()
    {
        _hitbox = GetComponent<Collider2D>();
    }

    public List<EnemyHealth> GetEnemiesInHitbox()
    {
        List<Collider2D> hits = new();
        ContactFilter2D filter = new() { useTriggers = true };
        Physics2D.OverlapCollider(_hitbox, filter, hits);

        List<EnemyHealth> enemies = new();
        foreach (var hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemies.Add(enemyHealth);
        }

        return enemies;
    }
}
