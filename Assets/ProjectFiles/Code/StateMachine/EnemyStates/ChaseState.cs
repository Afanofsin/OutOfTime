using DefaultNamespace;

namespace FSM.EnemyStates
{
    public class ChaseState : BaseState
    {
        private MeleeEnemyBase enemy;
        
        public override void OnEnter()
        {
            enemy.ResetAttackTimer();
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

        public ChaseState(MeleeEnemyBase Enemy)
        {
            enemy = Enemy;
        }
    }
}