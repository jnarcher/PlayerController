using UnityEngine;

public class GrapplePointController : MonoBehaviour
{
    [SerializeField] private Color ActiveColor;
    [SerializeField] private Color InactiveColor;

    public LayerMask PlayerLayer;

    private SpriteRenderer _sprite;
    private Transform _playerTransform;
    private PlayerStats PlayerStats => GameManager.Instance.PlayerStats;
    private PlayerStateMachine.Player _playerController;

    // tracks whether the player is in range of this grapple point
    public bool IsOn => _isOn;
    private bool _isOn;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("GrapplePointController: No object found with tag `Player`");
        _playerTransform = player.transform;
        _playerController = player.GetComponent<PlayerStateMachine.Player>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        CheckDistance();
        ChangeSprite();
    }

    private void CheckDistance()
    {
        bool newStatus = false;

        if (PlayerStats.GrappleToggle && Vector2.Distance(_playerTransform.position, transform.position) <= PlayerStats.GrappleRange)
        {
            Vector2 dir = (_playerTransform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, PlayerStats.GrappleRange, PlayerLayer);

            if ((bool)hit)
                newStatus = hit.collider.CompareTag("Player");
        }

        // only update status when it changes 
        if (newStatus != _isOn) SetStatus(newStatus);
    }

    private void SetStatus(bool newStatus)
    {
        _isOn = newStatus;

        if (_isOn)
            _playerController.AddActiveGrapplePoint(gameObject);
        else
            _playerController.RemoveActiveGrapplePoint(gameObject);

    }

    private void ChangeSprite()
    {
        _sprite.color = _isOn ? ActiveColor : InactiveColor;
    }
}
