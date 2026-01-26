using DefaultNamespace;
using UnityEngine;

namespace FSM.EnemyStates
{
    public class MeleeAttackState : BaseState
    {
        private MeleeEnemyBase enemy;
        private float timer;
        
        public override void OnEnter()
        {
            timer = 0;
            enemy.StopMovement();
            enemy.Action();
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            enemy.FaceTarget();
            enemy.StopMovement();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public bool IsAttackComplete() => timer > enemy.AttackDelay;

        public MeleeAttackState(MeleeEnemyBase Enemy)
        {
            enemy = Enemy;
        }
    }
}