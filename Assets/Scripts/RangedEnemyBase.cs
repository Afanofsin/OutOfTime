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
        [SerializeField] protected float attackRange = 8f;
        [SerializeField] protected float retreatDistance = 4f;
        [SerializeField] protected float strafeRadius = 2f;
        [SerializeField] protected float moveSpeed = 3f;
        
        [Header("Attack Settings")]
        public float attackCooldown = 1.5f;
        [SerializeField] protected BulletType bulletType;
    
        protected NavMeshAgent agent;
        protected SpriteRenderer sprite;
        protected Rigidbody2D rb;
        protected StateMachine stateMachine;
        protected float lastAttackTime;
        protected bool isInitialized = false;
        
        // Public properties for predicates
        public float AttackRange => attackRange;
        public float RetreatDistance => retreatDistance;
        
        /*protected EnemyInitState initState;
        protected EngageState engageState;
        protected AttackState attackState;
        protected RetreatState retreatState;
        protected DeadState deadState;*/
        
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

        public virtual void InitializeStateMachine()
        {
            /*stateMachine = new StateMachine();

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
            stateMachine.SetState(initState);*/
        }
        
        protected virtual void Update()
        {
            if (Target == null || !isInitialized) return;
            stateMachine.Update();
        }

        protected virtual void FixedUpdate()
        {
            if (Target == null || !isInitialized) return;
            stateMachine.FixedUpdate();
        }
        
        public virtual void MoveTowardsTarget()
        {
            if (Target == null) return;
            agent.SetDestination(Target.transform.position);
        }

        public virtual void MoveAwayFromTarget()
        {
            if (Target == null) return;
            
            Vector2 directionAway = (transform.position - Target.transform.position).normalized;
            Vector2 retreatPosition = (Vector2)transform.position + directionAway * retreatDistance;
        
            agent.SetDestination(retreatPosition);
        }
        
        public virtual void StrafeAroundTarget()
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
        
        public virtual float GetDistanceToTarget()
        {
            if (Target == null) return Mathf.Infinity;
            return Vector2.Distance(transform.position, Target.transform.position);
        }

        public virtual void FaceTarget()
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

        public virtual void OnDeath()
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
            base.React();
        }

        public override void SetTarget(GameObject targetGo)
        {
            base.SetTarget(targetGo);
            isInitialized = true;
        }

        public virtual void TakeDamage(IReadOnlyDictionary<DamageType, float> damage)
        {
            foreach (var damageKvp in damage)
            {
                CurrentHealth -= Mathf.Max(0, damageKvp.Value - damageKvp.Value * (resists[damageKvp.Key] / 100));
            }
            React();
        }


        /*public void Attack()
        {
            var instance = PoolManager.Instance.projectilePools[bulletType].Get();
            instance.Launch(Target.transform.position, 
                new Dictionary<DamageType, float>() { { DamageType.Physical, 10f } });
        }*/
    }
}