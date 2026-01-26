using ProjectFiles.Code.Boss;
using UnityEngine;
using UnityEngine.AI;

namespace FSM.EnemyStates.HobGoblin
{
    public class BossRidingState : BaseState
    {
        private BossRangedEnemy boss;
        private float shootTimer = 0f;
        private float ridingShootInterval = 0.5f;
        private bool movingRight = true;
        private Vector3 startPosition;

        public BossRidingState(BossRangedEnemy boss)
        {
            this.boss = boss;
        }

        public void OnEnter()
        {
            Debug.Log("Boss Riding State - Starting passes");
            boss.ResetRidingPass();
            boss.ResetStateTimer();
            startPosition = boss.transform.position;
            movingRight = true;
            shootTimer = 0f;
            
            // Set higher speed for riding
            boss.GetComponent<NavMeshAgent>().speed = boss.RidingSpeed;
            
            SetNextRidingDestination();
        }

        public void Update()
        {
            boss.FaceTarget();
            
            shootTimer += Time.deltaTime;
            
            // Shoot while riding
            if (shootTimer >= ridingShootInterval)
            {
                boss.Action();
                shootTimer = 0f;
            }

            // Check if reached destination
            if (boss.GetComponent<NavMeshAgent>().remainingDistance < 0.5f)
            {
                movingRight = !movingRight;
                boss.IncrementRidingPass();
                
                if (boss.CurrentRidingPass < 3)
                {
                    SetNextRidingDestination();
                }
            }
        }

        private void SetNextRidingDestination()
        {
            Vector3 destination;
            if (movingRight)
            {
                destination = startPosition + new Vector3(boss.ArenaWidth / 2, 0, 0);
            }
            else
            {
                destination = startPosition + new Vector3(-boss.ArenaWidth / 2, 0, 0);
            }
            
            boss.GetComponent<NavMeshAgent>().SetDestination(destination);
        }

        public void FixedUpdate() { }

        public void OnExit()
        {
            Debug.Log($"Boss Riding State Complete - {boss.CurrentRidingPass} passes");
        }
    }
}