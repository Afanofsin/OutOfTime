using UnityEngine;

namespace FSM.EnemyStates.Hog
{
    public class ChargingState : BaseState
    {
        private Enemies.Hog enemy;
        private Vector2 chargeVelocity;

        public ChargingState(Enemies.Hog enemy)
        {
            this.enemy = enemy;
        }

        public override void OnEnter()
        {
            Debug.Log($"{enemy.name}: CHARGING!");
            enemy.StartCharge();
        }

        public override void OnFixedUpdate()
        {
            enemy.UpdateCharge();
        }

        public override void OnUpdate()
        {
            enemy.FaceChargeDirection();
        }

        public override void OnExit()
        {
            enemy.StopCharge();
        }
    }
}