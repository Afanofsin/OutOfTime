using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsMediator
{
    private readonly LinkedList<StatModifier> _modifiers = new ();

    public event EventHandler<Query> Queries;

    public void PerformQuery(object sender, Query query) => Queries?.Invoke(sender, query);

    public void AddModifier(StatModifier statModifier)
    {
        _modifiers.AddLast(statModifier);
        Queries += statModifier.Handle;

        statModifier.OnDispose += _ =>
        {
            _modifiers.Remove(statModifier);
            Queries -= statModifier.Handle;
        };
    }

    public void Update()
    {
        var node = _modifiers.First;
        while (node != null)
        {
            var nextNode = node.Next;
            if (node.Value.MarkedForRemoval)
            {
                node.Value.Dispose();
            }
            node = nextNode;
        }
    }
}

public class Query
{
    public readonly StatType StatType;
    public readonly DamageType? DamageType;
    public float Value;

    public Query(StatType statType, float value, DamageType? damageType = null)
    {
        StatType = statType;
        Value = value;
        DamageType = damageType;
    }
}