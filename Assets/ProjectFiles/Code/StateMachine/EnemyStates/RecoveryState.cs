using DefaultNamespace;
using UnityEngine;

namespace FSM.EnemyStates
{
    public class RecoveryState : BaseState
    {
        private MeleeEnemyBase enemy;
        private float recoveryTimer;
        private bool hasBackstepped;
        
        public RecoveryState(MeleeEnemyBase Enemy)
        {
            enemy = Enemy;
        }

        public override void OnEnter()
        {
            recoveryTimer = 0f;
            hasBackstepped = false;
            enemy.ResetAttackTimer();
        }

        public override void OnUpdate()
        {
            recoveryTimer += Time.deltaTime;
            enemy.FaceTarget();
        }

        public override void OnFixedUpdate()
        {
            if (!hasBackstepped && recoveryTimer < 0.2f)
            {
                enemy.BackstepFromTarget();
                hasBackstepped = true;
            }
        }

        public override void OnExit()
        {
            recoveryTimer = 0f;
            hasBackstepped = false;
        }
        
        public bool IsRecoveryComplete() => recoveryTimer >= enemy.RecoveryDuration;
    }
}