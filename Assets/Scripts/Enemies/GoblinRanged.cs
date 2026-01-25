using System.Collections.Generic;
using DefaultNamespace;
using FSM;
using FSM.EnemyStates;
using FSM.EnemyStates.Predicates;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class GoblinRanged : EnemyEntityBase, IDamageable, IRangedEnemy
    {
        [Header("AI Settings")]
        [SerializeField] private float attackRange = 8f;
        [SerializeField] private float retreatDistance = 4f;
        [SerializeField] private float strafeRadius = 2f;
        [SerializeField] private float moveSpeed = 3f;
        
        [Header("Attack Settings")]
        public float attackCooldown = 1.5f;
        [SerializeField] protected BulletType bulletType;
    
        private NavMeshAgent agent;
        private SpriteRenderer sprite;
        private Rigidbody2D rb;
        private StateMachine stateMachine;
        private IState CurrentState => stateMachine.GetState();
        private float lastAttackTime;
        private float dynamicAttackCooldown;
        private bool isInitialized = false;
        
        // Public properties for predicates
        public float AttackRange
        {
            get { return attackRange; }
        }

        public float RetreatDistance
        {
            get { return retreatDistance; }
        }

        [SerializeField] private Dictionary<DamageType, float> Damage = new Dictionary<DamageType, float>()
            { { DamageType.Physical, 10f } };
        
        EnemyInitState initState;
        EngageState engageState;
        GoblinRangeAttackState attackState;
        GoblinRangeRetreatState retreatState;
        DeadState deadState;

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
            dynamicAttackCooldown = attackCooldown;
        
            InitializeStateMachine();
        }

        public void InitializeStateMachine()
        {
            stateMachine = new StateMachine();

            initState = new EnemyInitState(this);
            engageState = new EngageState(this);
            attackState = new GoblinRangeAttackState(this);
            retreatState = new GoblinRangeRetreatState(this);
            deadState = new DeadState(this);
            
            var isDead = new IsDeadPredicate(this);
            var isTooClose = new IsTooClosePredicate(this);
            var isInAttackRange = new IsInAttackRangePredicate(this);
            var isTooFar = new IsTooFarPredicate(this);
            
            stateMachine.AddAnyTransition(deadState, new FuncPredicate(
                () => CurrentHealth <= 0
            ));
            
            stateMachine.AddTransition(initState, engageState, new FuncPredicate(() =>
                {
                    bool result = Target != null;
                    Debug.Log($"{name}: InitState->EngageState predicate - Target is {(Target != null ? Target.name : "NULL")}, result: {result}");
                    return result;
                }
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
            
            stateMachine.SetState(initState);
        }
        
        public override void Action()
        {
            if (lastAttackTime < dynamicAttackCooldown) return;
        
            var instance = PoolManager.Instance.projectilePools[bulletType].Get();
            var dirToPlayer = (Target.transform.position - this.transform.position).normalized;
            instance.transform.position = this.transform.position + dirToPlayer + dirToPlayer + dirToPlayer;
            instance.Launch(dirToPlayer, Damage, LayerMask.GetMask("Damagable"));
        
            lastAttackTime = 0;
            dynamicAttackCooldown = attackCooldown * UnityEngine.Random.Range(1f, 1.8f);
        }
        
        private void Update()
        {
            if (Target == null || !isInitialized) return;
            lastAttackTime += Time.deltaTime;
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
        
        public override void React()
        {
            base.React();
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