using DefaultNamespace;
using Interfaces;

namespace FSM.EnemyStates.Predicates
{
    public class IsInAttackRangePredicate : IPredicate
    {
        private IRangedEnemy enemy;
        
        public IsInAttackRangePredicate(IRangedEnemy Enemy)
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