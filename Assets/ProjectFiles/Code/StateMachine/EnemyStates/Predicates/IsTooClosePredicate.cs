using DefaultNamespace;

namespace FSM.EnemyStates.Predicates
{
    public class IsTooClosePredicate : IPredicate
    {
        private RangedEnemyBase enemy;

        public IsTooClosePredicate(RangedEnemyBase Enemy)
        {
            enemy = Enemy;
        }

        public bool Evaluate() => enemy.GetDistanceToTarget() < enemy.RetreatDistance;

    }
}