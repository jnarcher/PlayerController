using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int StartHealth = 2;
    public int _currentHealth;

    private SpriteRenderer _sprite;

    private float _flashTimer;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Start() => _currentHealth = StartHealth;

    public void Damage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0) Destroy(gameObject);
        _flashTimer = 0.1f;
    }

    private void Update()
    {
        _flashTimer -= Time.deltaTime;

        if (_flashTimer > 0)
            _sprite.color = Color.white;
        else
            _sprite.color = Color.red;
    }

}
