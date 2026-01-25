using System.Collections.Generic;
using FSM;
using FSM.EnemyStates;
using FSM.EnemyStates.Hog;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Hog : EnemyEntityBase, IDamageable
    {
        [Header("Ramming Settings")]
        [SerializeField] protected float positioningSpeed = 2f; // Slow speed while aligning
        [SerializeField] protected float chargeSpeed = 12f; // Fast charge speed
        [SerializeField] protected float alignmentThreshold = 0.5f; // How close to axis to be "aligned"
    
        [Header("Charge Timing")]
        [SerializeField] protected float chargeWindupDuration = 0.8f;
        [SerializeField] protected float stunDuration = 1.2f;
    
        [Header("Charge Damage")]
        [SerializeField] protected float chargeDamage = 20f;
    
        [Header("Detection")]
        [SerializeField] protected LayerMask wallLayer;
        [SerializeField] protected float wallCheckDistance = 0.6f;
        
        protected NavMeshAgent agent;
        protected SpriteRenderer sprite;
        protected Rigidbody2D rb;
        protected StateMachine stateMachine;
        protected bool isInitialized = false;
        
        // Charge state
        protected Vector2 chargeDirection;
        protected bool isCharging = false;
        protected bool chargeHitSomething = false;
        protected Dictionary<DamageType, float> chargeDamageDict;
    
        // Public properties
        public float ChargeWindupDuration => chargeWindupDuration;
        public float StunDuration => stunDuration;
        
        private EnemyInitState initState;
        private PositioningState positioningState;
        private ChargeWindupState windupState;
        private ChargingState chargingState;
        private StunnedState stunnedState;
        private DeadState deadState;
        
        public override void Awake()
        {
            base.Awake();
        
            rb = GetComponent<Rigidbody2D>();
            agent = GetComponent<NavMeshAgent>();
            sprite = GetComponent<SpriteRenderer>();
        
            // Configure NavMeshAgent for 2D
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = positioningSpeed;
        
            // Setup charge damage
            chargeDamageDict = new Dictionary<DamageType, float>
            {
                { DamageType.Physical, chargeDamage }
            };
        
            isInitialized = false;
            
            chargeFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = wallLayer,
                useTriggers = false
            };
            
            InitializeStateMachine();
        }
        
        public void InitializeStateMachine()
        {
            stateMachine = new StateMachine();
            
            // Create states
            initState = new EnemyInitState(this);
            positioningState = new PositioningState(this);
            windupState = new ChargeWindupState(this);
            chargingState = new ChargingState(this);
            stunnedState = new StunnedState(this);
            deadState = new DeadState(this);
            
            // Death transition from any state
            stateMachine.AddAnyTransition(deadState, new FuncPredicate(
                () => CurrentHealth <= 0
            ));
            
            // Init -> Positioning (when target assigned)
            stateMachine.AddTransition(initState, positioningState, new FuncPredicate(
                () => Target != null
            ));
            
            // Positioning -> Windup (when aligned with player)
            stateMachine.AddTransition(positioningState, windupState, new FuncPredicate(
                () => IsAlignedWithPlayer()
            ));
            
            // Windup -> Charging (when windup complete)
            stateMachine.AddTransition(windupState, chargingState, new FuncPredicate(
                () => windupState.IsWindupComplete()
            ));
            
            // Windup -> Positioning (if player moves and we lose alignment)
            stateMachine.AddTransition(windupState, positioningState, new FuncPredicate(
                () => !IsAlignedWithPlayer()
            ));
            
            // Charging -> Stunned (when hit wall or player)
            stateMachine.AddTransition(chargingState, stunnedState, new FuncPredicate(
                () => HasChargeEnded()
            ));
            
            // Stunned -> Positioning (when stun complete, start over)
            stateMachine.AddTransition(stunnedState, positioningState, new FuncPredicate(
                () => stunnedState.IsStunComplete()
            ));
            
            // Start in init state
            stateMachine.SetState(initState);
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

        // Alignment Logic
        public bool IsAlignedWithPlayer()
        {
            if (Target == null) return false;
        
            Vector2 toTarget = Target.transform.position - transform.position;
            float absX = Mathf.Abs(toTarget.x);
            float absY = Mathf.Abs(toTarget.y);
        
            // Aligned if on same X or Y axis (within threshold)
            return absX < alignmentThreshold || absY < alignmentThreshold;
        }

        public void MoveToAlign()
        {
            if (Target == null) return;
        
            Vector2 toTarget = Target.transform.position - transform.position;
            float absX = Mathf.Abs(toTarget.x);
            float absY = Mathf.Abs(toTarget.y);
        
            Vector2 destination;
        
            // Move towards whichever axis is closer to align
            if (absX < absY)
            {
                // Move to align on X axis (same vertical line)
                destination = new Vector2(Target.transform.position.x, transform.position.y);
            }
            else
            {
                // Move to align on Y axis (same horizontal line)
                destination = new Vector2(transform.position.x, Target.transform.position.y);
            }
        
            agent.speed = positioningSpeed;
            agent.SetDestination(destination);
        }

        // Charge Logic
        public void DetermineChargeDirection()
        {
            if (Target == null) return;
        
            Vector2 toTarget = Target.transform.position - transform.position;
            float absX = Mathf.Abs(toTarget.x);
            float absY = Mathf.Abs(toTarget.y);
        
            // Choose cardinal direction based on player position
            if (absX > absY)
            {
                chargeDirection = toTarget.x > 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                chargeDirection = toTarget.y > 0 ? Vector2.up : Vector2.down;
            }
        }

        public void StartCharge()
        {
            isCharging = true;
            chargeHitSomething = false;
            agent.enabled = false; // Disable NavMesh during charge
            rb.linearVelocity = chargeDirection * chargeSpeed;
        }
        
        private readonly RaycastHit2D[] castResults = new RaycastHit2D[4];
        private ContactFilter2D chargeFilter;

        public void UpdateCharge()
        {
            float distance = chargeSpeed * Time.fixedDeltaTime;

            if (rb.Cast(chargeDirection, chargeFilter, castResults, distance) > 0)
            {
                rb.velocity = Vector2.zero;
                rb.MovePosition(rb.position + chargeDirection * (castResults[0].distance - 0.01f));
                isCharging = false;
            }
            else
            {
                rb.MovePosition(rb.position + chargeDirection * distance);
            }
        }

        public void StopCharge()
        {
            isCharging = false;
            rb.linearVelocity = Vector2.zero;
            agent.enabled = true;
        }

        public bool HasChargeEnded()
        {
            return chargeHitSomething;
        }

        public void StopMovement()
        {
            if (agent.enabled)
                agent.ResetPath();
            rb.linearVelocity = Vector2.zero;
        }

        public void FaceTarget()
        {
            if (Target == null || sprite == null) return;
            sprite.flipX = transform.position.x < Target.transform.position.x;
        }

        public void FaceChargeDirection()
        {
            if (sprite == null) return;
        
            // Face right if charging right, left otherwise
            if (chargeDirection == Vector2.right)
                sprite.flipX = true;
            else if (chargeDirection == Vector2.left)
                sprite.flipX = false;
        }

        // Collision handling
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isCharging) return;
        
            // Hit player during charge
            var damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null && collision.gameObject == Target)
            {
                damageable.TakeDamage(chargeDamageDict);
                Debug.Log($"{name}: Hit player during charge!");
                chargeHitSomething = true;
                return;
            }
        
            // Hit wall (fallback if raycast missed)
            if (((1 << collision.gameObject.layer) & wallLayer) != 0)
            {
                Debug.Log($"{name}: Collision with wall!");
                chargeHitSomething = true;
            }
        }

        public virtual void OnDeath()
        {
            if (agent.enabled)
                agent.enabled = false;
        }

        public override void SetTarget(GameObject targetGo)
        {
            base.SetTarget(targetGo);
            isInitialized = true;
        }

        
        public override void Action()
        {
            
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