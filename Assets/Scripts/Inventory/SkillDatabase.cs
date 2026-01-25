using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillDatabase : SerializedMonoBehaviour
{
    [SerializeField] private SkillDatabaseSO database;
    public static SkillDatabase Instance { get; private set; }

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

    public SkillBase GetSkillByID(int id)
    {
        if (database.skills.TryGetValue(id, out var skill))
        {
            return skill;
        }
        return database.skills[0];
    }
    
    public SkillBase GetRandomSkill() => database.skills[Random.Range(0, database.skills.Count)];
}
