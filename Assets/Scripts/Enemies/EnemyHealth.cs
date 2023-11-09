using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyHealth : Health
{
    public int StartHealth = 2;
    public bool CanBeDamaged = true;

    private int _currentHealth;
    private SpriteRenderer _sprite;
    private float _flashTimer;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Start() => _currentHealth = StartHealth;

    public override void Damage(int damage)
    {
        _flashTimer = 0.1f;
        if (CanBeDamaged)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0) Destroy(gameObject);
        }
    }

    public override void Kill()
    {
        Destroy(gameObject);
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
