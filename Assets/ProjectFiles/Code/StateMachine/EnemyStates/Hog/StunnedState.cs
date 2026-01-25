using UnityEngine;
using Enemies;

namespace FSM.EnemyStates
{
    public class StunnedState : BaseState
    {
        private Enemies.Hog enemy;
        private float stunTimer;

        public StunnedState(Enemies.Hog enemy)
        {
            this.enemy = enemy;
        }

        public override void OnEnter()
        {
            Debug.Log($"{enemy.name}: Stunned!");
            stunTimer = 0f;
            enemy.StopMovement();
            
            // TODO: Visual feedback (stars, dizzy effect)
        }

        public override void OnUpdate()
        {
            stunTimer += Time.deltaTime;
        }

        public override void OnExit()
        {
            stunTimer = 0f;
        }

        public bool IsStunComplete() => stunTimer >= enemy.StunDuration;
    }
}