using DefaultNamespace;

namespace FSM.EnemyStates
{
    public class MeleeAttackState : BaseState
    {
        private MeleeEnemyBase enemy;
        public override void OnEnter()
        {
            enemy.StopMovement();
            enemy.Action();
        }

        public override void OnUpdate()
        {
            enemy.FaceTarget();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public MeleeAttackState(MeleeEnemyBase Enemy)
        {
            enemy = Enemy;
        }
    }
}