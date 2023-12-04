using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerStats PlayerStats;
    public KnockbackCurves KnockbackCurves;

    public GameObject Player;
    public PlayerHealth PlayerHealth;

    public float HitFreezeDuration;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;
    }

    #region Player Health

    public void DamagePlayer(int damage, Vector2 direction, float knockbackStrength) => PlayerHealth.Damage(damage, direction, knockbackStrength);
    public void DamageAndRespawn(int damage) => PlayerHealth.DamageAndRespawn(damage); // for hazards
    public void KillPlayer() => PlayerHealth.Kill();

    #endregion

    public void HitFreeze() => StartCoroutine(DoFreeze());
    private IEnumerator DoFreeze()
    {
        float cachedTimeScale = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(HitFreezeDuration);
        Time.timeScale = cachedTimeScale;
    }
}
