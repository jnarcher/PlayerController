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

    public HitData FindGrapplePoint(Vector2 direction)
    {
        HitData data;
        Physics2D.queriesStartInColliders = false;

        if (_grapplePlayerIsInside != null)
        {
            // get the angle to the grapple point
            Vector2 directionToGrapplePoint = _grapplePlayerIsInside.transform.position - transform.position;
            float directionToGrapplePointAngle = Mathf.Rad2Deg * Mathf.Atan2(
                directionToGrapplePoint.y,
                directionToGrapplePoint.x
            );

            // get input direction angle
            float inputAngle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);

            // compare this angle to the input angle
            bool hit = false;
            if (Mathf.Abs(directionToGrapplePointAngle - inputAngle) <= _stats.CloseupGrappleAssistAngle) // TODO: this is an aim assist tolerance and should be editable within a script
                hit = true;

            data = new()
            {
                DidHit = hit,
                HitPosition = _grapplePlayerIsInside.transform.position,
                HitObject = _grapplePlayerIsInside
            };
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(
                _col.bounds.center,
                direction,
                _stats.GrappleRange,
                GrappleLayer
            );

            bool didHit = (bool)hit;
            Vector2 pos = didHit ? hit.transform.position : Vector2.zero;
            GameObject obj = didHit ? hit.collider.gameObject : null;


            data = new()
            {
                DidHit = didHit,
                HitPosition = pos,
                HitObject = obj
            };

        }

        Physics2D.queriesStartInColliders = _cachedQueriesStartInColliders;
        return data;
    }

    private GameObject _grapplePlayerIsInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("GrapplePoint")) _grapplePlayerIsInside = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GrapplePoint")) _grapplePlayerIsInside = null;
    }
}

public struct HitData
{
    public bool DidHit;
    public Vector2 HitPosition;
    public GameObject HitObject;
}