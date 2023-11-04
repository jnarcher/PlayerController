using System;
using UnityEditor.ShortcutManagement;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerCollision : MonoBehaviour
{

    [SerializeField] private float _groundCheckDistance = 0.05f;
    [SerializeField] private float _ceilingCheckDistance = 0.05f;
    [SerializeField] private float _wallCheckDistance = 0.05f;

    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private LayerMask GrappleLayer;

    public bool OnGround { get; private set; }
    public bool OnCeiling { get; private set; }
    public bool OnWall { get; private set; }

    private PlayerController _controller;

    private BoxCollider2D _col;
    private PlayerStats _stats;
    private bool _cachedQueriesStartInColliders;

    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        _controller = GetComponent<PlayerController>();
        _stats = _controller.Stats;
        _cachedQueriesStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        Physics2D.queriesStartInColliders = false;

        OnGround = Physics2D.BoxCast(
            _col.bounds.center,
            _col.size,
            0f,
            Vector2.down,
            _groundCheckDistance,
            PlayerLayer
        );

        OnCeiling = Physics2D.BoxCast(
            _col.bounds.center,
            _col.size,
            0f,
            Vector2.up,
            _ceilingCheckDistance,
            PlayerLayer
        );

        OnWall = Physics2D.BoxCast(
            _col.bounds.center,
            _col.size,
            0f,
            _controller.IsFacingRight ? Vector2.right : Vector2.left,
            _wallCheckDistance,
            PlayerLayer
        );

        Physics2D.queriesStartInColliders = _cachedQueriesStartInColliders;
    }

    public RaycastHit2D FindGrapplePoint(Vector2 direction)
    {
        Physics2D.queriesStartInColliders = false;

        Debug.DrawRay(_col.bounds.center, _stats.GrappleRange * direction, Color.green, 10f);

        RaycastHit2D hit = Physics2D.BoxCast(
            _col.bounds.center,
            _col.size,
            0f,
            direction,
            _stats.GrappleRange,
            GrappleLayer
        );

        Physics2D.queriesStartInColliders = _cachedQueriesStartInColliders;
        return hit;
    }
}
