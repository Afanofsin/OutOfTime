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

        public override void OnEnter()
        {
            Debug.Log("Boss Init State");
        }

        public override void OnUpdate() { }
        public override void OnFixedUpdate() { }
        public override void OnExit() { }
    }
}