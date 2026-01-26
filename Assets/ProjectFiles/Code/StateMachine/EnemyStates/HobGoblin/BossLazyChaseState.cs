using ProjectFiles.Code.Boss;
using UnityEngine;
using UnityEngine.AI;

namespace FSM.EnemyStates.HobGoblin
{
    public class BossLazyChaseState : BaseState
    {
        private BossRangedEnemy boss;
        private float shootTimer = 0f;

        public BossLazyChaseState(BossRangedEnemy boss)
        {
            this.boss = boss;
        }

        public override void OnEnter()
        {
            Debug.Log("Boss Lazy Chase State");
            boss.ResetStateTimer();
            shootTimer = 0f;
            
            // Set slower speed for lazy chase
            boss.GetComponent<NavMeshAgent>().speed = boss.LazyChaseSpeed;
        }

        public override void OnUpdate()
        {
            boss.FaceTarget();
            
            float distanceToTarget = boss.GetDistanceToTarget();
            
            // Move towards player if too far, but stop if too close
            if (distanceToTarget > boss.LazyChaseDistance)
            {
                boss.MoveTowardsTarget();
            }
            else
            {
                // Stop moving if within lazy chase distance
                boss.GetComponent<NavMeshAgent>().ResetPath();
            }

            // Shoot periodically
            shootTimer += Time.deltaTime;
            if (shootTimer >= boss.attackCooldown)
            {
                boss.Action();
                shootTimer = 0f;
            }
        }

        public override void OnFixedUpdate() { }

        public override void OnExit()
        {
            Debug.Log("Boss Lazy Chase State Complete - Cycling back to Riding");
        }
    }
}