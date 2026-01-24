using DefaultNamespace;
using Interfaces;

namespace FSM.EnemyStates
{
    public class EngageState : BaseState
    {
        private IRangedEnemy enemy;
        
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnUpdate()
        {
            enemy.FaceTarget();
        }

        public override void OnFixedUpdate()
        {
            enemy.MoveTowardsTarget();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public EngageState(IRangedEnemy Enemy)
        {
            enemy = Enemy;
        }
    }
}