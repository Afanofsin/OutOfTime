using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    private Projectile _prefab;
    [SerializeField] private int initialSize = 32;
    public static ProjectilePool Instance;
    private readonly Queue<Projectile> pool = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
        
    }

    private void Warmup()
    {
        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }

    private Projectile CreateNew()
    {
        var p = Instantiate(_prefab, transform);
        p.gameObject.SetActive(false);
        p.SetPool(this);
        pool.Enqueue(p);
        return p;
    }

    public Projectile Get()
    {
        if (pool.Count == 0)
            CreateNew();

        return pool.Dequeue();
    }

    public void Release(Projectile projectile)
    {
        pool.Enqueue(projectile);
    }

    public void SetPrefab(Projectile prefab)
    {
        _prefab = prefab;
    } 
    
    public void Clear()
    {
        while (pool.Count > 0)
        {
            var p = pool.Dequeue();
            if (p != null)
                Destroy(p.gameObject);
        }
    }
}