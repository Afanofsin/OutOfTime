using UnityEngine;

namespace FSM.EnemyStates.Hog
{
    public class ChargeWindupState : BaseState
    {
        private Enemies.Hog enemy;
        private float windupTimer;

        public ChargeWindupState(Enemies.Hog enemy)
        {
            this.enemy = enemy;
        }

        public override void OnEnter()
        {
            Debug.Log($"{enemy.name}: Winding up charge");
            windupTimer = 0f;
            enemy.StopMovement();
            enemy.DetermineChargeDirection();
        }

        public override void OnUpdate()
        {
            windupTimer += Time.deltaTime;
            enemy.FaceChargeDirection();
            
            // TODO: Visual telegraph (sprite flash, particles, etc.)
        }

        public override void OnExit()
        {
            windupTimer = 0f;
        }

        public bool IsWindupComplete() => windupTimer >= enemy.ChargeWindupDuration;
    }
}