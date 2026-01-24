using DefaultNamespace;
using UnityEngine;

namespace FSM.EnemyStates
{
    public class AttackState : BaseState
    {
        
        private RangedEnemyBase enemy;
        
        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
           
        }

        public override void OnFixedUpdate()
        {
            
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