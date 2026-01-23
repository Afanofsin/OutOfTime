using DefaultNamespace;
using UnityEngine;

namespace FSM.EnemyStates
{
    public class AttackState : BaseState
    {
        private float strafeTimer;
        private float strafeInterval = 1f;
        private RangedEnemyBase enemy;
        
        public override void OnEnter()
        {
            strafeTimer = 0;
        }

        public override void OnUpdate()
        {
            enemy.FaceTarget();
            
            // TODO: Perform Attack
        }

        public override void OnFixedUpdate()
        {
            strafeTimer += Time.deltaTime;
            if (strafeTimer >= strafeInterval)
            {
                enemy.StrafeAroundTarget();
                strafeTimer = 0;
            }
            
            
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public AttackState(RangedEnemyBase Enemy)
        {
            enemy = Enemy;
        }
    }
}