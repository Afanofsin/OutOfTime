using System.Collections.Generic;
using FSM;
using FSM.EnemyStates;
using FSM.EnemyStates.Predicates;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class RangedEnemyBase : EnemyEntityBase, IDamageable
    {
        [Header("AI Settings")]
        [SerializeField] private float attackRange = 8f;
        [SerializeField] private float retreatDistance = 4f;
        [SerializeField] private float strafeRadius = 2f;
        [SerializeField] private float moveSpeed = 3f;
        
        [Header("Attack Settings")]
        [SerializeField] private float attackCooldown = 1.5f;
    
        private NavMeshAgent agent;
        private SpriteRenderer sprite;
        private Rigidbody2D rb;
        private StateMachine stateMachine;
        private float lastAttackTime;
        private bool isInitialized = false;
        
        // Public properties for predicates
        public float AttackRange => attackRange;
        public float RetreatDistance => retreatDistance;
        
        private EnemyInitState initState;
        private EngageState engageState;
        private AttackState attackState;
        private RetreatState retreatState;
        private DeadState deadState;
        
        public override void Awake()
        {
            base.Awake();
        
            // Setup components
            rb = GetComponent<Rigidbody2D>();
            agent = GetComponent<NavMeshAgent>();
            sprite = GetComponent<SpriteRenderer>();
        
            // Configure NavMeshAgent for 2D
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed;
            isInitialized = false;
        
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            stateMachine = new StateMachine();

            initState = new EnemyInitState();
            engageState = new EngageState(this);
            attackState = new AttackState(this);
            retreatState = new RetreatState(this);
            deadState = new DeadState(this);
            
            var isDead = new IsDeadPredicate(this);
            var isTooClose = new IsTooClosePredicate(this);
            var isInAttackRange = new IsInAttackRangePredicate(this);
            var isTooFar = new IsTooFarPredicate(this);
            
            stateMachine.AddAnyTransition(deadState, isDead);
        
            stateMachine.AddTransition(initState, engageState, new FuncPredicate(
                    () => Target != null
                ));
            
            // Engage -> Attack (when in range)
            stateMachine.AddTransition(engageState, attackState, isInAttackRange);
        
            // Attack -> Retreat (when too close)
            stateMachine.AddTransition(attackState, retreatState, isTooClose);
        
            // Attack -> Engage (when too far)
            stateMachine.AddTransition(attackState, engageState, isTooFar);
        
            // Retreat -> Attack (when back in attack range)
            stateMachine.AddTransition(retreatState, attackState, isInAttackRange);
        
            // Retreat -> Engage (when far enough after retreating)
            stateMachine.AddTransition(retreatState, engageState, isTooFar);
        
            // Start in Engage state (enemies are immediately aggroed)
            stateMachine.SetState(initState);
        }
        
        private void Update()
        {
            if (Target == null || !isInitialized) return;
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            if (Target == null || !isInitialized) return;
            stateMachine.FixedUpdate();
        }
        
        public void MoveTowardsTarget()
        {
            if (Target == null) return;
            agent.SetDestination(Target.transform.position);
        }

        public void MoveAwayFromTarget()
        {
            if (Target == null) return;
            
            Vector2 directionAway = (transform.position - Target.transform.position).normalized;
            Vector2 retreatPosition = (Vector2)transform.position + directionAway * retreatDistance;
        
            agent.SetDestination(retreatPosition);
        }
        
        public void StrafeAroundTarget()
        {
            if (Target == null) return;
        
            // Calculate perpendicular direction for strafing
            Vector2 toTarget = Target.transform.position - transform.position;
            Vector2 strafeDirection = Vector2.Perpendicular(toTarget).normalized;
        
            // Randomly choose left or right strafe
            if (Random.value > 0.5f)
                strafeDirection = -strafeDirection;
        
            Vector2 strafePosition = (Vector2)transform.position + strafeDirection * strafeRadius;
            agent.SetDestination(strafePosition);
        }
        
        public float GetDistanceToTarget()
        {
            if (Target == null) return Mathf.Infinity;
            return Vector2.Distance(transform.position, Target.transform.position);
        }

        public void FaceTarget()
        {
            // To the Right
            if (this.transform.position.x < Target.transform.position.x)
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
            }
        }

        public void OnDeath()
        {
            agent.enabled = false;
            // Additional death logic here
        }

        // Attack method placeholder
        public override void Action()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
        
            // TODO: Implement ranged attack
            // Example: Instantiate projectile, play animation, etc.
        
            lastAttackTime = Time.time;
        }
        
        public override void React()
        {
            
        }

        public override void SetTarget(GameObject targetGo)
        {
            base.SetTarget(targetGo);
            isInitialized = true;
        }

        public void TakeDamage(IReadOnlyDictionary<DamageType, float> damage)
        {
            foreach (var damageKvp in damage)
            {
                CurrentHealth -= Mathf.Max(0, damageKvp.Value - damageKvp.Value * (resists[damageKvp.Key] / 100));
            }
            React();
        }
    }
}