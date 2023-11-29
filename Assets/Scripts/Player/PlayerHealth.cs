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

    private void Update()
    {
        HealthText.text = _currentHealth.ToString();
    }

    public override void Damage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0) Kill();
    }

    public void DamageAndRespawn(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
            Kill();
        else
            Player.Respawn();

    }

    public override void Kill()
    {
        Debug.Log("Player has died.");
        _currentHealth = Stats.MaxHealth; // ! only for debugging
        Player.Respawn();
    }

}
