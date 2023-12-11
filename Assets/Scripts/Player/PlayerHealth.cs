using PlayerStateMachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public Collider2D Hurtbox;
    public Text HealthText;
    private int _currentHealth;

    private Player Player;

    private PlayerStats Stats => GameManager.Instance.PlayerStats;

    private void Start()
    {
        _currentHealth = Stats.MaxHealth;
        Player = GetComponentInParent<Player>();
    }

    public override void Damage(int damage, Vector2 direction, float knockbackStrength)
    {
        _currentHealth -= damage;
        CameraShakeManager.Instance.CameraShake(GameManager.Instance.PlayerStats.HitCameraShakeIntensity);
        ControllerRumbleManager.Instance.SetRumblePulse(1f, 0.1f);
        if (_currentHealth <= 0)
            Kill();
        else
        {
            GameManager.Instance.HitFreeze(0.2f);
            Player.Hit(direction);
        }
    }

    public void DamageAndRespawn(int damage, Vector2 direction)
    {
        _currentHealth -= damage;
        CameraShakeManager.Instance.CameraShake(GameManager.Instance.PlayerStats.HitCameraShakeIntensity);
        ControllerRumbleManager.Instance.SetRumblePulse(1f, 0.1f);
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
        Player.Kill();
        GameManager.Instance.RespawnPlayer();
    }

}
