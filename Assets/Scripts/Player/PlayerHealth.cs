using PlayerStateMachine;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerHealth : Health
{
    public Collider2D Hurtbox;
    public Text HealthText;
    private int _currentHealth;

    private Player Player;

    private CinemachineImpulseSource _impulseSource;

    private PlayerStats Stats => GameManager.Instance.PlayerStats;

    private void Start()
    {
        _currentHealth = Stats.MaxHealth;
        Player = GetComponentInParent<Player>();
        _impulseSource = GetComponentInParent<CinemachineImpulseSource>();
    }

    private void Update()
    {
    }

    public override void Damage(int damage, Vector2 direction, float knockbackStrength)
    {
        _currentHealth -= damage;
        CameraShakeManager.Instance.CameraShake(_impulseSource, GameManager.Instance.PlayerStats.HitCameraShakeIntensity);
        if (_currentHealth <= 0)
            Kill();
        else
        {
            GameManager.Instance.HitFreeze();
            Player.Hit(direction);
        }
    }

    public void DamageAndRespawn(int damage, Vector2 direction)
    {
        _currentHealth -= damage;
        CameraShakeManager.Instance.CameraShake(_impulseSource, GameManager.Instance.PlayerStats.HitCameraShakeIntensity);
        if (_currentHealth <= 0)
            Kill();
        else
        {
            Player.Hit(direction);
            GameManager.Instance.RespawnPlayer();
        }

    }

    public override void Kill()
    {
        Debug.Log("Player has died.");
        _currentHealth = Stats.MaxHealth;
        GameManager.Instance.RespawnPlayer();
    }

}
