using DefaultNamespace;

namespace FSM.EnemyStates.Predicates
{
    public class IsDeadPredicate : IPredicate
    {
        private RangedEnemyBase enemy;
        
        public IsDeadPredicate(RangedEnemyBase Enemy)
        {
            enemy = Enemy;
        }

        public bool Evaluate() => enemy.CurrentHealth <= 0;
    }
}