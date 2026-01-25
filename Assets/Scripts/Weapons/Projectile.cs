using System;
using System.Collections.Generic;
using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : SerializedMonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private BulletType pool;
    private Vector2 _direction;
    private Dictionary<DamageType, float> _damage;
    private Rigidbody2D _rb;
    private Collider2D _collider2D;
    private float _timer;
    private ProjectilePool _pool;
    private LayerMask _mask;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }
    
    private void Start()
    {
        SetPool(PoolManager.Instance.projectilePools[pool]);
    }

    internal void SetPool(ProjectilePool owner)
    {
        _pool = owner;
    }

    public void Launch(Vector2 direction, Dictionary<DamageType, float> damage, LayerMask mask)
    {
        _mask = mask;
        _collider2D.excludeLayers += _mask;
        _damage = damage;
        _timer = 0f;
        trail.Clear();
        _direction = direction;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
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
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
        } 
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
        _collider2D.excludeLayers -= _mask;
        _pool.Release(this);
    }
}