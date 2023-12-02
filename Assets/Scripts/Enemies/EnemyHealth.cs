using StaticEnemy;
using UnityEngine;

public class EnemyHealth : Health
{
    public bool CanBeDamaged = true;
    public float InvincibilityTime = 0.2f;
    private IEnemyController _controller;
    private EnemyStats _stats;

    private int _currentHealth;
    private SpriteRenderer _sprite;
    private float _flashTimer;

    private float _invincibilityTimer;

    private void Awake()
    {
        _sprite = GetComponentInParent<SpriteRenderer>();
    }

    private void Start()
    {
        _controller = GetComponentInParent<IEnemyController>();
        _stats = _controller.GetStats();
        _currentHealth = _stats.MaxHealth;
    }

    public override void Damage(int damage, Vector2 knockback)
    {
        if (CanBeDamaged)
        {
            _flashTimer = 0.1f;
            _invincibilityTimer = InvincibilityTime;
            _currentHealth -= damage;
            CanBeDamaged = false;
            _controller.Knockback(knockback);
            _controller.Stun();
            if (_currentHealth <= 0) Kill();
        }
    }

    public override void Kill() => _controller.Kill();

    public void AirLaunch(bool toRight) => _controller.AirLaunch(toRight);

    private void Update()
    {
        _invincibilityTimer -= Time.deltaTime;
        _flashTimer -= Time.deltaTime;

        if (_invincibilityTimer < 0)
            CanBeDamaged = true;

        if (_flashTimer > 0)
            _sprite.color = Color.white;
        else
            _sprite.color = Color.red;
    }
}
