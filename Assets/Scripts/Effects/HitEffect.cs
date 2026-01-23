using System;
using UnityEngine;

public enum HitEffectType
{
    Blood,
    GreenBlood,
    Sand,
    PurpleBlood
}


public class HitEffect : MonoBehaviour
{
    [SerializeField] private float lifetime;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private HitEffectType pool;
    private HitPool _pool;
    private float _timer;
    private Vector2 _direction;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        SetPool(PoolManager.Instance.pools[pool]);
    }

    private void OnEnable()
    {
        particles.Play();
    }

    internal void SetPool(HitPool owner)
    {
        _pool = owner;
    }
    
    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        if (_timer >= lifetime)
        {
            ReturnToPool();
        }
    }
    
    private void ReturnToPool()
    {
        _timer = 0;
        gameObject.SetActive(false);
        _pool.Release(this);
    }
}
