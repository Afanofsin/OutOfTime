using DefaultNamespace;
using Interfaces;

namespace FSM.EnemyStates
{
    public class DeadState : BaseState
    {
        private EnemyEntityBase enemy;
        
        public override void OnEnter()
        {
            enemy.Die();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public DeadState(EnemyEntityBase Enemy)
        {
            enemy = Enemy;
        }
    }
}