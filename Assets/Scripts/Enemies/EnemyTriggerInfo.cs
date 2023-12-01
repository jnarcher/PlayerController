using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerInfo : MonoBehaviour
{
    public LayerMask PlayerLayer;
    public LayerMask LineOfSightLayers;
    public LayerMask GroundLayer;
    public Collider2D SearchZone;
    public Collider2D LedgeCheck;

    public bool CanSeePlayer { get; private set; }
    public bool IsNearLedge { get; private set; }

    private void FixedUpdate()
    {
        CheckSearchZone();
        CheckLedge();
    }

    private void CheckSearchZone()
    {
        // Check if player is within search zone
        ContactFilter2D filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = PlayerLayer,
        };

        List<Collider2D> cols = new();
        int hits = Physics2D.OverlapCollider(SearchZone, filter, cols);

        if (hits == 0)
        {
            CanSeePlayer = false;
            return;
        }

        // Check line of sight
        Vector2 ray = GameManager.Instance.Player.transform.position - transform.position;
        RaycastHit2D rayData = Physics2D.Raycast(transform.position, ray.normalized, ray.magnitude, LineOfSightLayers);

        if ((bool)rayData)
            CanSeePlayer = rayData.collider.CompareTag("Player");
    }

    private void CheckLedge()
    {
        ContactFilter2D filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = GroundLayer,
        };
        int hits = Physics2D.OverlapCollider(LedgeCheck, filter, new List<Collider2D>());
        IsNearLedge = hits == 0;
    }
}
