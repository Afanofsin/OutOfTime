using DefaultNamespace;
using Interfaces;

namespace FSM.EnemyStates.Predicates
{
    public class IsDeadPredicate : IPredicate
    {
        private IRangedEnemy enemy;
        
        public IsDeadPredicate(IRangedEnemy Enemy)
        {
            enemy = Enemy;
        }

        public bool Evaluate() => false; //enemy.CurrentHealth <= 0;
    }
}