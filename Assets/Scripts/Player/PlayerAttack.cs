using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerAttack : MonoBehaviour
{
    public LayerMask EnemyLayer;
    public Collider2D AttackCollider;
    public GameObject AttackSprite;

    private List<GameObject> _hitColliders = new();

    private void Update()
    {
        if (_attackPressedThisFrame)
        {
            AttackSprite.SetActive(true);
            var hits = GetHitColliders();

            foreach (var col in hits)
                col.gameObject.GetComponent<EnemyHealth>().Damage(1);
        }
        else
        {
            AttackSprite.SetActive(false);
        }
        _attackPressedThisFrame = false;
    }

    private List<Collider2D> GetHitColliders()
    {
        ContactFilter2D filter = new()
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = EnemyLayer
        };

        List<Collider2D> hitColliders = new();
        Physics2D.OverlapCollider(AttackCollider, filter, hitColliders);
        return hitColliders;
    }

    private bool _attackPressedThisFrame;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            _attackPressedThisFrame = true;
    }
}
