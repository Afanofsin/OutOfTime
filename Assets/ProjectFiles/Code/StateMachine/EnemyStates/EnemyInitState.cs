using UnityEngine;

namespace FSM.EnemyStates
{
    public class EnemyInitState : BaseState
    {
        private EnemyEntityBase enemy;
        
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
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            
        }

        public EnemyInitState(EnemyEntityBase Enemy)
        {
            enemy = Enemy;
        }
    }
}