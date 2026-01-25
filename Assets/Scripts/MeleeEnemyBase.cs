using System.Collections.Generic;
using FSM;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class MeleeEnemyBase : EnemyEntityBase, IDamageable
    {
        [Header("Melee AI Settings")]
        [SerializeField] protected float meleeRange = 1.5f;
        [SerializeField] protected float moveSpeed = 4f;
        [SerializeField] protected float backstepDistance = 1f;
        
        [Header("Attack Timing")]
        [SerializeField] protected float attackDelayMin = 0.5f; 
        [SerializeField] protected float attackDelayMax = 1.2f; 
        [SerializeField] protected float windupDuration = 0.3f; 
        [SerializeField] protected float recoveryDuration = 0.5f;
        [SerializeField] protected float attackDuration = 0.5f;
        
        [SerializeField] protected float personalSpaceRadius = 1.5f;
        [SerializeField] protected LayerMask enemyLayer;
        
        protected NavMeshAgent agent;
        protected SpriteRenderer sprite;
        protected Rigidbody2D rb;
        protected StateMachine stateMachine;
        protected IState CurrentState => stateMachine.GetState();
        protected bool isInitialized = false;

        private float windupTimer = 0f;
        private float requiredAttackDelay = 0.5f;
    
        // Public properties for predicates
        public float MeleeRange => meleeRange;
        public float WindupDuration => windupDuration;
        public float RecoveryDuration => recoveryDuration;
        public float AttackDelay => attackDuration * 2;

        public override void Awake()
        {
            base.Awake();
        
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

        public abstract void InitializeStateMachine();

        protected virtual void Update()
        {
            Debug.Log(CurrentState);
            if (Target == null || !isInitialized) return;
            stateMachine.Update();
        }

        protected virtual void FixedUpdate()
        {
            if (Target == null || !isInitialized) return;
            stateMachine.FixedUpdate();
        
            // Apply spacing behavior to avoid stacking
            ApplyPersonalSpace();
        }

        // Movement Methods
        public virtual void MoveTowardsTarget()
        {
            if (Target == null) return;
        
            // Check if there's an enemy too close in our path
            Vector2 desiredPosition = GetSpacedPosition(Target.transform.position);
            agent.SetDestination(desiredPosition);
        }

        public virtual void StopMovement()
        {
            if (agent.enabled)
                agent.ResetPath();
        }

        public virtual void BackstepFromTarget()
        {
            if (Target == null) return;
        
            Vector2 directionAway = (transform.position - Target.transform.position).normalized;
            Vector2 backstepPosition = (Vector2)transform.position + directionAway * backstepDistance;
        
            agent.SetDestination(backstepPosition);
        }

        public virtual float GetDistanceToTarget()
        {
            if (Target == null) return Mathf.Infinity;
            return Vector2.Distance(transform.position, Target.transform.position);
        }

        public virtual void FaceTarget()
        {
            if (Target == null || sprite == null) return;
        
            // Flip sprite based on target direction
            sprite.flipX = transform.position.x < Target.transform.position.x;
        }

        // Personal space system to prevent enemies from stacking
        private void ApplyPersonalSpace()
        {
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
                transform.position, 
                personalSpaceRadius, 
                enemyLayer
            );

            Vector2 avoidanceOffset = Vector2.zero;
            
            foreach (var other in nearbyEnemies)
            {
                if (other.gameObject == gameObject) continue;
            
                Vector2 away = (transform.position - other.transform.position).normalized;
                float distance = Vector2.Distance(transform.position, other.transform.position);
            
                if (distance < personalSpaceRadius * 0.7f)
                {
                    avoidanceOffset += away * 0.5f;
                }
            }
            
            // Apply offset to current destination
            if (avoidanceOffset.sqrMagnitude > 0.01f && agent.hasPath)
            {
                agent.SetDestination((Vector2)agent.destination + avoidanceOffset);
            }
        }

        private Vector2 GetSpacedPosition(Vector2 targetPosition)
        {
            // Check if there's already an enemy near the target
            Collider2D[] nearTarget = Physics2D.OverlapCircleAll(
                targetPosition,
                meleeRange * 1.5f,
                enemyLayer
            );

            // If others are attacking, position ourselves at a slight angle
            if (nearTarget.Length > 1)
            {
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * meleeRange;
                return targetPosition + offset;
            }

            return targetPosition;
        }
        
        public void ResetAttackTimer()
        {
            windupTimer = 0f;
            requiredAttackDelay = Random.Range(attackDelayMin, attackDelayMax);
        }

        public void IncrementAttackTimer(float deltaTime)
        {
            windupTimer += deltaTime;
        }

        public bool CanAttack()
        {
            return windupTimer >= requiredAttackDelay;
        }

        public virtual void OnDeath()
        {
            agent.enabled = false;
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