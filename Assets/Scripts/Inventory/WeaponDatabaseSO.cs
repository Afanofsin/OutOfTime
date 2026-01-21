using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Database/WeaponDatabase")]
public class WeaponDatabaseSO : SerializedScriptableObject
{
    [ValidateInput(nameof(HasUniqueInts), "Weapon IDs must be unique")]
    public Dictionary<int, WeaponBase> weapons;

    private bool HasUniqueInts(Dictionary<int, WeaponBase> dict)
    {
        if (dict == null)
            return true;

        return dict.Values.Distinct().Count() == dict.Values.Count;
    }
}