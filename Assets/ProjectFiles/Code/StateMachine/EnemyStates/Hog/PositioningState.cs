using UnityEngine;

namespace FSM.EnemyStates.Hog
{
    public class PositioningState : BaseState
    {
        private Enemies.Hog enemy;

        public PositioningState(Enemies.Hog enemy)
        {
            this.enemy = enemy;
        }

        public override void OnEnter()
        {
            Debug.Log($"{enemy.name}: Positioning to align with player");
        }

        public override void OnFixedUpdate()
        {
            enemy.MoveToAlign();
        }

        public override void OnUpdate()
        {
            enemy.FaceTarget();
        }
    }
}