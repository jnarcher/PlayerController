using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Collider2D Hitbox;
    public Text HealthText;
    private int _currentHealth;

    private PlayerStats Stats => GameManager.Instance.PlayerStats;

    private void Start()
    {
        _currentHealth = Stats.MaxHealth;
    }

    private void Update()
    {
        HealthText.text = _currentHealth.ToString();
    }

    public void Damage(int damage)
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
            Respawn();

    }

    public void Respawn()
    {
        GameObject checkpoint = CheckpointManager.Instance.GetRespawnCheckpoint();
        transform.position = checkpoint.transform.GetChild(0).position;
    }

    public void Kill()
    {
        Debug.Log("Player has died.");
        _currentHealth = Stats.MaxHealth; // ! only for debugging
        Respawn();
    }

}
