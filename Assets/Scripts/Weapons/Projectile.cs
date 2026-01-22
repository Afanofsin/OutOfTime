using System;
using System.Collections.Generic;
using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : SerializedMonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    private Vector2 _direction;
    private Dictionary<DamageType, float> _damage;
    private Rigidbody2D _rb;
    private float _timer;
    private ProjectilePool _pool;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    internal void SetPool(ProjectilePool owner)
    {
        _pool = owner;
    }

    public void Launch(Vector2 direction, Dictionary<DamageType, float> damage)
    {
        _damage = damage;
        _timer = 0f;
        _direction = direction;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        _rb.linearVelocity = _direction.normalized * speed;
    }

    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        if (_timer >= lifetime)
        {
            ReturnToPool();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IEnemy>(out var enemy))
        {
            if (enemy is IDamageable damageable)
            { 
                damageable.TakeDamage(_damage);
                ReturnToPool();
            }
        } 
    }

    private void ReturnToPool()
    {
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
        _pool.Release(this);
    }
}