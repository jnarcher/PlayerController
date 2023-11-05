using UnityEngine;

public class GrapplePointController : MonoBehaviour
{
    [SerializeField] private Color ActiveColor;
    [SerializeField] private Color InactiveColor;

    private SpriteRenderer _sprite;
    private Transform _playerTransform;
    private PlayerStats _playerStats;

    private bool _cachedQueriesStartInColliders;

    // tracks whether the player is in range of this grapple point
    public bool IsOn => _isOn;
    private bool _isOn;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("GrapplePointController: No object found with tag `Player`");
        _playerTransform = player.transform;
        _playerStats = player.GetComponent<PlayerController>().Stats;
        _sprite = GetComponent<SpriteRenderer>();
        _cachedQueriesStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        CheckDistance();
        ChangeSprite();
    }

    private void CheckDistance()
    {
        Vector2 dir = (transform.position - _playerTransform.position).normalized;

        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(_playerTransform.position, dir, _playerStats.GrappleRange);
        Physics2D.queriesStartInColliders = _cachedQueriesStartInColliders;

        if ((bool)hit) SetStatus(hit.transform.gameObject == gameObject || _playerInsideCollider);
        else SetStatus(_playerInsideCollider);
    }

    private void SetStatus(bool newStatus)
    {
        if (!_playerStats.GrappleToggle)
        {
            _isOn = newStatus;
            return;
        }

        _isOn = newStatus;
    }

    private void ChangeSprite() => _sprite.color = _isOn ? ActiveColor : InactiveColor;

    private bool _playerInsideCollider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerInsideCollider = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerInsideCollider = false;
    }
}
