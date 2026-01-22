using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Scriptable Objects/EnemyDatabase")]
public class EnemyDatabase : SerializedScriptableObject
{
    [OdinSerialize] private Dictionary<string, EnemyEntityBase> enemies;

    public EnemyEntityBase GetEnemyByName(string name)
    {
        enemies.TryGetValue(name, out var enemy);
        return enemy;
    }
    
}
