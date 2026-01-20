using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponDatabase : SerializedMonoBehaviour
{
    [SerializeField] private WeaponDatabaseSO database;
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
}