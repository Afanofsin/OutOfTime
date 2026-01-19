using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerStats
{
    private readonly StatsMediator _mediator;
    private readonly BaseStats _baseStats;

    public StatsMediator Mediator => _mediator;
    
    public Dictionary<DamageType, float> Attack
    {
        get
        {
            var result = new Dictionary<DamageType, float>();

            foreach (var dmg in _baseStats.attack)
            {
                var q = new Query(StatType.Attack, dmg.Value, dmg.Key);
                _mediator.PerformQuery(this, q);

                result[dmg.Key] = q.Value;
            }

            return result;
        }
    }

    public float AttackSpeed
    {
        get
        {
            var q = new Query(StatType.AttackSpeed, _baseStats.attackSpeed);
            _mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    
    public float Speed
    {
        get
        {
            var q = new Query(StatType.Speed, _baseStats.speed);
            _mediator.PerformQuery(this, q);
            return q.Value;
        }
    }
    
    public float DashRange
    {
        get;
    }

    public Dictionary<DamageType, float> Resists
    {
        get;
    }

    public PlayerStats(BaseStats baseStats, StatsMediator mediator)
    {
        _mediator = mediator;
        _baseStats = baseStats;
    }
}