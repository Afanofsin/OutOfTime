using System.Collections.Generic;
using Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public abstract class EnemyEntityBase : EntityBase, IEnemy
{
    [SerializeField, ReadOnly] private GameObject target;
    public GameObject Target => target;
    
    public void SetTarget(GameObject targetGo)
    {
        target = targetGo;
    }
    
    public abstract void Action();
}
