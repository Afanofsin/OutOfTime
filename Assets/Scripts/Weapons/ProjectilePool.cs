using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile prefab;
    [SerializeField] private int initialSize = 32;
    private readonly Queue<Projectile> _pool = new();
    
    private void Awake()
    {
        Warmup();
    }

    private void Warmup()
    {
        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }

    private Projectile CreateNew()
    {
        var p = Instantiate(prefab, transform);
        p.gameObject.SetActive(false);
        p.SetPool(this);
        _pool.Enqueue(p);
        return p;
    }

    public Projectile Get()
    {
        if (_pool.Count == 0)
            CreateNew();

        return _pool.Dequeue();
    }

    public void Release(Projectile projectile)
    {
        _pool.Enqueue(projectile);
    }

    public void SetPrefab(Projectile prefab)
    {
        this.prefab = prefab;
    } 
    
    public void Clear()
    {
        while (_pool.Count > 0)
        {
            var p = _pool.Dequeue();
            if (p != null)
                Destroy(p.gameObject);
        }
    }
}