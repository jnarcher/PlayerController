using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int CurrentHealth;

    private PlayerStats Stats => GameManager.Instance.PlayerStats;

    private void Start()
    {
        CurrentHealth = Stats.MaxHealth;
    }

    public void Damage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) Kill();
    }

    public void DamageAndRespawn(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
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
        CurrentHealth = Stats.MaxHealth; // ! only for debugging
        Respawn();
    }

}
