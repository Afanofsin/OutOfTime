using DefaultNamespace;

namespace FSM.EnemyStates
{
    public class RetreatState : BaseState
    {
        private RangedEnemyBase enemy;
        
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

        public RetreatState(RangedEnemyBase Enemy)
        {
            enemy = Enemy;
        }
    }
}