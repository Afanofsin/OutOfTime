using Enemies;
using Interfaces;
using UnityEngine;

namespace FSM.EnemyStates
{
    public class GoblinRangeRetreatState : RetreatState
    {
        private GoblinRanged enemy;
        private float attackTimer;
        public override void OnEnter()
        {
            base.OnEnter();
            attackTimer = 0;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            enemy.Action();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
        public GoblinRangeRetreatState(GoblinRanged Enemy) : base(Enemy)
        {
            enemy = Enemy;
        }
    }
}