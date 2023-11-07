using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds information about the various triggers used by the player controller.
/// </summary>
public class TriggerInfo : MonoBehaviour
{

    [SerializeField] private LayerMask PlayerLayer;

    [Space]
    [SerializeField] private BoxCollider2D _groundCheck;
    [SerializeField] private BoxCollider2D _wallCheck;

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

        ContactFilter2D filter = new() { useLayerMask = true, layerMask = PlayerLayer };
        List<Collider2D> hits = new();

        // this will always be at least 1 since it will hit the player itself
        bool groundHit = _groundCheck.OverlapCollider(filter, hits) > 1;
        bool wallHit = _wallCheck.OverlapCollider(filter, hits) > 1;


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
}
