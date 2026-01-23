using DefaultNamespace;

namespace FSM.EnemyStates.Predicates
{
    public class IsInAttackRangePredicate : IPredicate
    {
        private RangedEnemyBase enemy;
        
        public IsInAttackRangePredicate(RangedEnemyBase Enemy)
        {
            enemy = Enemy;
        }

        public bool Evaluate()
        {
            float distance = enemy.GetDistanceToTarget();
            return distance >= enemy.RetreatDistance && distance <= enemy.AttackRange;
        }
    }
}