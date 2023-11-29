using PlayerStateMachine;
using UnityEngine;

public class GrapplePointController : MonoBehaviour
{
    [SerializeField] private Color ActiveColor;
    [SerializeField] private Color InactiveColor;
    [SerializeField] private Color CooldownColor;

    public LayerMask PlayerLayer;
    public float GrapplePointCooldown;

    private SpriteRenderer _sprite;
    private Transform _playerTransform;
    private PlayerStats PlayerStats => GameManager.Instance.PlayerStats;
    private Player _playerController;

    private bool _outOfCooldown = true;
    private float _cooldownTimer;
    private GameObject _indicator;

    // tracks whether the player is in range of this grapple point
    public bool IsOn => _isOn;
    private bool _isOn;

    private void Start()
    {
        GameObject player = GameManager.Instance.Player;
        _playerTransform = player.transform;
        _playerController = player.GetComponent<Player>();
        _sprite = GetComponent<SpriteRenderer>();
        _indicator = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        SetTargeted();
        ChangeSprite();
    }

    private void FixedUpdate()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer < 0)
            _outOfCooldown = true;

        CheckDistance();
    }

    private void CheckDistance()
    {
        bool newStatus = false;

        if (_outOfCooldown && PlayerStats.GrappleToggle && Vector2.Distance(_playerTransform.position, transform.position) <= PlayerStats.GrappleRange)
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
        if (_outOfCooldown)
            _sprite.color = _isOn ? ActiveColor : InactiveColor;
        else
            _sprite.color = CooldownColor;
    }

    public void StartCooldown()
    {
        _outOfCooldown = false;
        _cooldownTimer = GrapplePointCooldown;
    }

    private void SetTargeted()
    {
        _indicator.SetActive(
            _isOn &&
            _playerController.StateType == PlayerStateType.GrappleAim &&
            _playerController.SelectedGrapplePoint == gameObject
        );
    }
}
