using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "BoonDatabase", menuName = "Database/BoonDatabase")]
public class BoonDatabaseSO : SerializedScriptableObject
{
    public List<BoonBase> boons;
}