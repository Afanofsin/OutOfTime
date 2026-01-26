using System.Collections.Generic;
using DefaultNamespace;
using FSM;
using FSM.EnemyStates;
using FSM.EnemyStates.Predicates;
using UnityEngine;

namespace Enemies
{
    public class MeleeGoblin : MeleeEnemyBase
    {
        [Header("Goblin Melee Specific")]
        [SerializeField] private float meleeDamage = 15f;
        [SerializeField] private ISwing swing;
        
        private Dictionary<DamageType, float> damage = new Dictionary<DamageType, float>()
            { { DamageType.Physical, 10f } };
        
        private EnemyInitState initState;
        private ChaseState chaseState;
        private WindupState windupState;
        private MeleeAttackState attackState;
        private RecoveryState recoveryState;
        private DeadState deadState;
        
        public override void Awake()
        {
            base.Awake();
        }
        
        public override void InitializeStateMachine()
        {
            stateMachine = new StateMachine();
            
            // Create states
            initState = new EnemyInitState(this);
            chaseState = new ChaseState(this);
            windupState = new WindupState(this);
            attackState = new MeleeAttackState(this);
            recoveryState = new RecoveryState(this);
            //deadState = new DeadState(this);
            
            // Create predicates
            //var isDead = new IsDeadPredicate(this);
            
            // Death transition from any state
            //stateMachine.AddAnyTransition(deadState, isDead);
            
            // Init -> Chase (when target assigned)
            stateMachine.AddTransition(initState, chaseState, new FuncPredicate(
                () => Target != null
            ));
            
            // Chase -> Windup (when in range AND enough time has passed)
            stateMachine.AddTransition(chaseState, windupState, new FuncPredicate(() =>
                {
                    bool isInRange = GetDistanceToTarget() < meleeRange;
                    return isInRange; //&& CanAttack();
                }
            ));
            
            // Windup -> Attack (when windup complete)
            stateMachine.AddTransition(windupState, attackState, new FuncPredicate(
                () => windupState.IsWindupComplete() 
                    ));
            
            // Windup -> Chase (if player moves out of range during windup)
            stateMachine.AddTransition(windupState, chaseState, new FuncPredicate(
                () => GetDistanceToTarget() > meleeRange 
            ));
            
            // Attack -> Recovery (immediately after attack)
            stateMachine.AddTransition(attackState, recoveryState, new FuncPredicate(
                () => true
                ));
            
            // Recovery -> Chase (when recovery complete)
            stateMachine.AddTransition(recoveryState, chaseState, new FuncPredicate(
                () => recoveryState.IsRecoveryComplete()
            ));
            
            // Start in init state
            stateMachine.SetState(initState);
        }
        
        public override void Action()
        {
            Vector2 direction = (transform.position - Target.transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
            angle = angle < 0 ? angle + 360f : angle;
            swing.StartSwing(angle, damage, attackDuration);
        }
        
    }
}