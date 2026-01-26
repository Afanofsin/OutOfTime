using ProjectFiles.Code.Boss;
using UnityEngine;

namespace FSM.EnemyStates.HobGoblin
{
    public class BossInitState : BaseState
    {
        private BossRangedEnemy boss;

        public BossInitState(BossRangedEnemy boss)
        {
            this.boss = boss;
        }

        public void OnEnter()
        {
            Debug.Log("Boss Init State");
        }

        public void Update() { }
        public void FixedUpdate() { }
        public void OnExit() { }
    }
}