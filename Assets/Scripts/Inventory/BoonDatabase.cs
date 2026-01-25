using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoonDatabase : SerializedMonoBehaviour
{
    [SerializeField] private BoonDatabaseSO database;
    [SerializeField] public Dictionary<RarityType, float> rarityWeight;
    public static BoonDatabase Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public BoonBase GetWeaponByRarity(RarityType rarity)
    {
        
        List<BoonBase> candidates = new();

        foreach (var weapon in database.boons)
        {
            if (weapon.BoonRarity == rarity)
            {
                candidates.Add(weapon);
            }
        }

        if (candidates.Count == 0)
        {
            return null;  
        }
        
        return candidates[Random.Range(0, candidates.Count)];
    }
}
