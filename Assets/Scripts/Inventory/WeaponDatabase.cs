using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponDatabase : SerializedMonoBehaviour
{
    [SerializeField] private WeaponDatabaseSO database;
    [SerializeField] public Dictionary<RarityType, float> rarityWeight;
    public static WeaponDatabase Instance { get; private set; }
    
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

    public WeaponBase GetWeaponByID(int id)
    {
        if (database.weapons.TryGetValue(id, out var weapon))
        {
            return weapon;
        }
        return database.weapons[0];
    }

    public WeaponBase GetRandomWeapon() => database.weapons[Random.Range(0, database.weapons.Count)];

    public WeaponBase GetWeaponByRarity(RarityType rarity)
    {
        
        List<WeaponBase> candidates = new();

        foreach (var weapon in database.weapons.Values)
        {
            if (weapon.rarity == rarity)
            {
                candidates.Add(weapon);
            }
        }

        if (candidates.Count == 0)
        {
            return GetWeaponByID(0);  
        }
        
        return candidates[Random.Range(0, candidates.Count)];
    }
}