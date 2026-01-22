using DefaultNamespace;

namespace FSM.EnemyStates.Predicates
{
    public class IsTooFarPredicate : IPredicate
    {
        private RangedEnemyBase enemy;

        public IsTooFarPredicate(RangedEnemyBase Enemy)
        {
            enemy = Enemy;
        }
        
        public bool Evaluate() => enemy.GetDistanceToTarget() > enemy.AttackRange;
    }
}