using DefaultNamespace;
using UnityEngine;

namespace FSM.EnemyStates
{
    public class WindupState : BaseState
    {
        private MeleeEnemyBase enemy;
        private float timer;
        
        public override void OnEnter()
        {
            timer = 0;
            enemy.StopMovement();
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            enemy.StopMovement();
            enemy.FaceTarget();
            enemy.IncrementAttackTimer(Time.deltaTime);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            timer = 0f;
        }
        
        public bool IsWindupComplete() => timer >= enemy.WindupDuration;

        public WindupState(MeleeEnemyBase Enemy)
        {
            enemy = Enemy;
        }
    }
}