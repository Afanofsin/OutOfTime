using Enemies;
using UnityEngine;

namespace FSM.EnemyStates
{
    public class GoblinRangeAttackState : BaseState
    {
        private float strafeTimer;
        private float strafeInterval = 1f;
        private GoblinRanged enemy;
        
        public override void OnEnter()
        {
            strafeTimer = 0;
        }

        public override void OnUpdate()
        {
            enemy.Action();
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
        
        public GoblinRangeAttackState(GoblinRanged Enemy)
        {
            enemy = Enemy;
        }
    }
}