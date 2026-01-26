using DefaultNamespace;
using Interfaces;

namespace FSM.EnemyStates.Predicates
{
    public class IsTooClosePredicate : IPredicate
    {
        private IRangedEnemy enemy;

        public IsTooClosePredicate(IRangedEnemy Enemy)
        {
            enemy = Enemy;
        }

        public bool Evaluate() => enemy.GetDistanceToTarget() < enemy.RetreatDistance;

    }
}