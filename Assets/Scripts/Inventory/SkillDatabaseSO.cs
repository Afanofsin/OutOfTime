using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Database/SkillDatabase")]
public class SkillDatabaseSO : SerializedScriptableObject
{
    [ValidateInput(nameof(HasUniqueInts), "Skill IDs must be unique")]
    public Dictionary<int, SkillBase> skills;

    private bool HasUniqueInts(Dictionary<int, SkillBase> dict)
    {
        if (dict == null) return true;
        return dict.Values.Distinct().Count() == dict.Values.Count;
    }
}
