using DefaultNamespace;

namespace FSM.EnemyStates
{
    public class DeadState : BaseState
    {
        private RangedEnemyBase enemy;
        
        public override void OnEnter()
        {
            enemy.OnDeath();
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

        public DeadState(RangedEnemyBase Enemy)
        {
            enemy = Enemy;
        }
    }
}