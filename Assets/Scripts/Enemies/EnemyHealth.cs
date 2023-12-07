using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class EnemyHealth : Health
{
    private IEnemyController _controller;
    public EnemyStats Stats;

    private int _currentHealth;
    private SpriteRenderer _sprite;

    public bool HasTakenDamage { get; set; }

    private CinemachineImpulseSource _impulseSource;

    private void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _controller = GetComponentInParent<IEnemyController>();
        _sprite = GetComponentInParent<SpriteRenderer>();
        _currentHealth = Stats.MaxHealth;
    }

    public override void Damage(int damage, Vector2 direction, float knockbackStrength)
    {
        if (Stats.Damageable)
        {
            CameraShakeManager.Instance.CameraShake(_impulseSource, Stats.HitCameraShakeIntensity);
            GameManager.Instance.HitFreeze();
            HasTakenDamage = true;
            _currentHealth -= damage;
            _controller.DirectionHitFrom = direction;
            _controller.HitStrength = knockbackStrength;
            _controller.Stun();
            if (_currentHealth <= 0) Kill();
        }
    }

    public override void Kill() => _controller.Kill();

    public void AirLaunch(bool toRight) => _controller.AirLaunch(toRight);

    private void Update() { }
}
