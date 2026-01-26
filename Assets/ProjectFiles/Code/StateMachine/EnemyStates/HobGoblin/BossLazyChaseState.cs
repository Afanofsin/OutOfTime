using ProjectFiles.Code.Boss;
using UnityEngine;
using UnityEngine.AI;

namespace FSM.EnemyStates.HobGoblin
{
    public class BossLazyChaseState : BaseState
    {
        private BossRangedEnemy boss;
        private float shootTimer = 0f;
        private float lazyChaseShootInterval = 1f;

        public BossLazyChaseState(BossRangedEnemy boss)
        {
            this.boss = boss;
        }

        public void OnEnter()
        {
            Debug.Log("Boss Lazy Chase State");
            boss.ResetStateTimer();
            shootTimer = 0f;
            
            // Set slower speed for lazy chase
            boss.GetComponent<NavMeshAgent>().speed = boss.LazyChaseSpeed;
        }

        public void Update()
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
            if (shootTimer >= lazyChaseShootInterval)
            {
                boss.Action();
                shootTimer = 0f;
            }
        }

        public void FixedUpdate() { }

        public void OnExit()
        {
            Debug.Log("Boss Lazy Chase State Complete - Cycling back to Riding");
        }
    }
}