using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds information about the various triggers used by the player controller.
/// </summary>
public class TriggerInfo : MonoBehaviour
{

    [SerializeField] private LayerMask GroundLayer;

    [Space]
    [SerializeField] private BoxCollider2D _groundCheck;
    [SerializeField] private BoxCollider2D _wallCheck;


    [Header("Hitboxes")]
    public Collider2D PlayerHurtbox;
    public Collider2D GroundAttack1;
    public Collider2D GroundAttack2;
    public Collider2D AirAttack1;
    public Collider2D AirAttack2;
    public Collider2D SlideAttack;
    public Collider2D UpAttack;
    public Collider2D DownAttack;
    public Collider2D GrappleAttack;

    public bool OnGround { get; private set; }
    public bool OnWall { get; private set; }
    public float TimeLeftGround { get; private set; } = float.MinValue;
    public float TimeLeftWall { get; private set; } = float.MinValue;

    public bool LeftGroundThisFrame => _time == TimeLeftGround;
    public bool LeftWallThisFrame => _time == TimeLeftWall;

    public bool LandedThisFrame { get; private set; }
    public bool HitWallThisFrame { get; private set; }

    private float _time;

    private void Update() => _time += Time.deltaTime;

    private void FixedUpdate()
    {
        LandedThisFrame = false;
        HitWallThisFrame = false;

        ContactFilter2D filter = new() { useLayerMask = true, layerMask = GroundLayer };
        List<Collider2D> hits = new();

        bool groundHit = _groundCheck.OverlapCollider(filter, hits) > 0;
        bool wallHit = _wallCheck.OverlapCollider(filter, hits) > 0;

        // player just left ground
        if (!groundHit && OnGround)
            TimeLeftGround = _time;

        // player just left wall
        if (!wallHit && OnWall)
            TimeLeftWall = _time;

        // player just landed on ground
        if (groundHit && !OnGround)
            LandedThisFrame = true;

        // player just hit the wall
        if (wallHit && !OnWall)
            HitWallThisFrame = true;

        OnGround = groundHit;
        OnWall = wallHit;
    }

    public List<EnemyHealth> GetEnemiesInHitbox(Collider2D collider)
    {
        List<Collider2D> hits = new();
        ContactFilter2D filter = new()
        {
            useTriggers = true,
        };
        Physics2D.OverlapCollider(collider, filter, hits); // TODO: use air attack hitbox

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
