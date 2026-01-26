using ProjectFiles.Code.Boss;
using UnityEngine;
using UnityEngine.AI;

namespace FSM.EnemyStates.HobGoblin
{
    public class BossVulnerableState : BaseState
    {
        private BossRangedEnemy boss;

        public BossVulnerableState(BossRangedEnemy boss)
        {
            this.boss = boss;
        }

        public override void OnEnter()
        {
            Debug.Log("Boss Vulnerable State - Open for attacks!");
            boss.ResetStateTimer();
            
            // Stop moving
            boss.GetComponent<NavMeshAgent>().isStopped = true;
            
            // Optional: Play vulnerable animation
            if (boss.animator != null)
            {
                boss.animator.SetBool("IsVulnerable", true);
            }
        }

        public override void OnUpdate()
        {
            boss.FaceTarget();
            // Just wait, do nothing - vulnerable to attacks
        }

        public override void OnFixedUpdate() { }

        public override void OnExit()
        {
            Debug.Log("Boss Vulnerable State Complete");
            boss.GetComponent<NavMeshAgent>().isStopped = false;
            
            if (boss.animator != null)
            {
                boss.animator.SetBool("IsVulnerable", false);
            }
        }
    }
}