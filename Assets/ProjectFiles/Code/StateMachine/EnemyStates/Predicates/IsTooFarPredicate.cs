using DefaultNamespace;
using Interfaces;

namespace FSM.EnemyStates.Predicates
{
    public class IsTooFarPredicate : IPredicate
    {
        private IRangedEnemy enemy;

        public IsTooFarPredicate(IRangedEnemy Enemy)
        {
            enemy = Enemy;
        }
        
        public bool Evaluate() => enemy.GetDistanceToTarget() > enemy.AttackRange;
    }
}