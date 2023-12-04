using UnityEngine;
using Cinemachine;

public class EnemyHealth : Health
{
    private IEnemyController _controller;
    private EnemyStats _stats;

    private int _currentHealth;
    private SpriteRenderer _sprite;
    private float _flashTimer;

    public bool HasTakenDamage { get; set; }

    private CinemachineImpulseSource _impulseSource;

    private void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _controller = GetComponentInParent<IEnemyController>();
        _sprite = GetComponentInParent<SpriteRenderer>();
        _stats = _controller.GetStats();
        _currentHealth = _stats.MaxHealth;
    }

    public override void Damage(int damage, Vector2 direction, float knockbackStrength)
    {
        if (_stats.Damageable)
        {
            CameraShakeManager.Instance.CameraShake(_impulseSource);
            _flashTimer = 0.1f;
            _currentHealth -= damage;
            HasTakenDamage = true;

            _controller.DirectionHitFrom = direction;
            _controller.HitStrength = knockbackStrength;

            GameManager.Instance.HitFreeze();

            _controller.Stun();
            if (_currentHealth <= 0) Kill();
        }
    }

    public override void Kill() => _controller.Kill();

    public void AirLaunch(bool toRight) => _controller.AirLaunch(toRight);

    private void Update()
    {
        _flashTimer -= Time.deltaTime;

        if (_flashTimer > 0)
            _sprite.color = Color.white;
        else
            _sprite.color = Color.red;
    }
}
