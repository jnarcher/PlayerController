using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerStats PlayerStats;

    public GameObject Player;
    public PlayerHealth PlayerHealth;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    #region Player Health

    public void DamagePlayer(int damage) => PlayerHealth.Damage(damage);
    public void DamageAndRespawn(int damage) => PlayerHealth.DamageAndRespawn(damage); // for hazards
    public void KillPlayer() => PlayerHealth.Kill();

    #endregion
}
