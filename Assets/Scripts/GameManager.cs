using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerStats PlayerStats;

    private GameObject _player;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance);
        else
            Instance = this;

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerHealth = _player.GetComponent<PlayerHealth>();
    }

    #region Player Health

    public void DamagePlayer(int damage) => _playerHealth.Damage(damage);
    public void DamageAndRespawn(int damage) => _playerHealth.DamageAndRespawn(damage); // for hazards
    public void KillPlayer() => _playerHealth.Kill();
    public void RespawnPlayer() => _playerHealth.Respawn();

    #endregion
}
