using System.Collections.Generic;
using UnityEngine;

public class HitPool : MonoBehaviour
{
    [SerializeField] private HitEffect prefab;
    [SerializeField] private int initialSize = 32;

    private readonly Queue<HitEffect> _pool = new();

    private void Awake()
    {
        Warmup();
    }

    private void Warmup()
    {
        for (var i = 0; i < initialSize; i++)
        {
            CreateNew();
        }
    }

    private HitEffect CreateNew()
    {
        var obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
        return obj;
    }

    public HitEffect Get()
    {
        if (_pool.Count == 0)
            CreateNew();

        var obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(HitEffect obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}