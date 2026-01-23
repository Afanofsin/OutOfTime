using System.Collections.Generic;
using Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public abstract class EnemyEntityBase : EntityBase, IEnemy
{
    [SerializeField, ReadOnly] private GameObject target;
    [OdinSerialize] private Dictionary<DamageType, float> _damage;
    public Dictionary<DamageType, float> Damage => _damage;
    public GameObject Target => target;
    public virtual void SetTarget(GameObject targetGo) => target = targetGo;
    public abstract void Action();
}
