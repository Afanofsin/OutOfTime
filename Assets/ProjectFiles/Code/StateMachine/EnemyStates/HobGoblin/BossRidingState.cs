using ProjectFiles.Code.Boss;
using UnityEngine;
using UnityEngine.AI;

namespace FSM.EnemyStates.HobGoblin
{
    public class BossRidingState : BaseState
    {
        private BossRangedEnemy boss;
        private float shootTimer = 0f;
        private NavMeshAgent agent;

        // Arena corner points
        private Vector3 topLeft;
        private Vector3 topRight;
        private Vector3 bottomLeft;
        private Vector3 bottomRight;

        // Current riding state
        private enum Lane
        {
            Top,
            Bottom
        }

        private Lane currentLane;
        private bool movingRight;
        private Vector3 currentDestination;
        private bool isMovingToStartPosition;

        public BossRidingState(BossRangedEnemy boss)
        {
            this.boss = boss;
        }

        public override void OnEnter()
        {
            Debug.Log("Boss Riding State - Starting passes");
            boss.ResetRidingPass();
            boss.ResetStateTimer();
            shootTimer = 0f;

            agent = boss.GetComponent<NavMeshAgent>();
            agent.isStopped = false;
            agent.speed = boss.RidingSpeed;

            InitializeArenaPoints();
            DetermineStartingLaneAndDirection();

            // First move to the starting corner
            isMovingToStartPosition = true;
            MoveToStartingCorner();
        }

        private void InitializeArenaPoints()
        {
            // Option 1: Calculate from arena center and dimensions
            Vector3 arenaCenter = boss.spawnPoint;
            float halfWidth = boss.ArenaWidth / 1.85f;
            float halfDepth = boss.ArenaWidth / 1.85f;

            topLeft = arenaCenter + new Vector3(-halfWidth, halfDepth, 0);
            topRight = arenaCenter + new Vector3(halfWidth, halfDepth, 0);
            bottomLeft = arenaCenter + new Vector3(-halfWidth, -halfDepth, 0 );
            bottomRight = arenaCenter + new Vector3(halfWidth, -halfDepth, 0);

            Debug.Log($"Arena Points - TL: {topLeft}, TR: {topRight}, BL: {bottomLeft}, BR: {bottomRight}");
        }

        private void DetermineStartingLaneAndDirection()
        {
            Vector3 currentPos = boss.transform.position;

            // Find closest corner
            Vector3[] corners = { topLeft, topRight, bottomLeft, bottomRight };
            float minDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < corners.Length; i++)
            {
                float dist = Vector3.Distance(currentPos, corners[i]);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestIndex = i;
                }
            }

            // Set lane and direction based on closest corner
            switch (closestIndex)
            {
                case 0:
                    currentLane = Lane.Top;
                    movingRight = true;
                    break;
                case 1: 
                    currentLane = Lane.Top;
                    movingRight = false;
                    break;
                case 2: 
                    currentLane = Lane.Bottom;
                    movingRight = true;
                    break;
                case 3:
                    currentLane = Lane.Bottom;
                    movingRight = false;
                    break;
            }

            Debug.Log($"Starting Lane: {currentLane}, Moving Right: {movingRight}");
        }

        private void MoveToStartingCorner()
        {
            // Move to the corner we're starting from
            currentDestination = GetCurrentCorner();
            agent.SetDestination(currentDestination);
            Debug.Log($"Moving to starting corner: {currentDestination}");
        }

        private Vector3 GetCurrentCorner()
        {
            if (currentLane == Lane.Top)
                return movingRight ? topLeft : topRight;
            else
                return movingRight ? bottomLeft : bottomRight;
        }

        private Vector3 GetTargetCorner()
        {
            if (currentLane == Lane.Top)
                return movingRight ? topRight : topLeft;
            else
                return movingRight ? bottomRight : bottomLeft;
        }

        public override void OnUpdate()
        {
            boss.FaceTarget();

            // Only shoot while actively riding (not moving to start position)
            if (!isMovingToStartPosition)
            {
                shootTimer += Time.deltaTime;
                boss.animator.SetBool("IsStrafing", true);

                if (shootTimer >= boss.ridingShootInterval)
                {
                    boss.ShootWhileRiding();
                    shootTimer -= boss.ridingShootInterval;
                }
            }

            if (HasReachedDestination())
            {
                boss.animator.SetBool("IsStrafing", false);
                if (isMovingToStartPosition)
                {
                    // Reached starting corner, begin actual riding
                    isMovingToStartPosition = false;
                    SetNextRidingDestination();
                    Debug.Log("Reached starting corner, beginning ride!");
                }
                else
                {
                    // Completed a pass
                    boss.IncrementRidingPass();
                    Debug.Log($"Completed pass {boss.CurrentRidingPass}/3");

                    if (boss.CurrentRidingPass < 3)
                    {
                        // Reverse direction for next pass
                        movingRight = !movingRight;
                        SetNextRidingDestination();
                    }
                }
            }
        }

        private bool HasReachedDestination()
        {
            if (agent.pathPending) return false;
            return agent.remainingDistance <= agent.stoppingDistance + 0.1f;
        }

        private void SetNextRidingDestination()
        {
            currentDestination = GetTargetCorner();

            // Validate destination is on NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(currentDestination, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log(
                    $"Riding to: {hit.position} (Lane: {currentLane}, Direction: {(movingRight ? "Right" : "Left")})");
            }
            else
            {
                Debug.LogError($"Destination {currentDestination} is not on NavMesh!");
            }
        }

        public override void OnFixedUpdate()
        {
        }

        public override void OnExit()
        {
            Debug.Log($"Boss Riding State Complete - {boss.CurrentRidingPass} passes");
        }

    }
}