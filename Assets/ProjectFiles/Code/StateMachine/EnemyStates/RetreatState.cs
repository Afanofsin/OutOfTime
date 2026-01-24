using DefaultNamespace;
using Interfaces;

namespace FSM.EnemyStates
{
    public class RetreatState : BaseState
    {
        private IRangedEnemy enemy;
        
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnFixedUpdate()
        {
            enemy.MoveAwayFromTarget();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public RetreatState(IRangedEnemy Enemy)
        {
            enemy = Enemy;
        }
    }
}