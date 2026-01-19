using System;

public class StatModifierBase : StatModifier
{
    private readonly StatType _statType;
    private readonly DamageType? _damageType;
    private readonly Func<float, float> _operation;

    public StatModifierBase(StatType statType, Func<float, float> operation, DamageType? damageType = null)
    {
        _statType = statType;
        _operation = operation;
        _damageType = damageType;
    }
    
    public override void Handle(object sender, Query query)
    {
        if (query.StatType != _statType)
            return;

        if (_damageType == null || query.DamageType == _damageType)
        {
            query.Value = _operation(query.Value);
        }
    }
}


public abstract class StatModifier : IDisposable
{
    public bool MarkedForRemoval { get; private set; }
    public event Action<StatModifier> OnDispose = delegate { };
    public abstract void Handle(object sender, Query query);

    public void Dispose()
    {
        MarkedForRemoval = true;
        OnDispose?.Invoke(this);
    }
}