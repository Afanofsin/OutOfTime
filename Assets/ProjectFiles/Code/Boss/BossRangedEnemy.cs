using System.Collections.Generic;
using DefaultNamespace;
using FSM;
using FSM.EnemyStates;
using FSM.EnemyStates.HobGoblin;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectFiles.Code.Boss
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BossRangedEnemy : RangedEnemyBase, IDamageable
    {
        [Header("Boss Specific Settings")]
        [SerializeField] private float ridingSpeed = 6f;
        [SerializeField] public float ridingShootInterval = 0.5f;
        [SerializeField] private float vulnerableStateDuration = 3f;
        [SerializeField] private float lazyChaseSpeed = 2f;
        [SerializeField] private float lazyChaseDistance = 6f; // Won't get closer than this
        [SerializeField] private float lazyChaseShootInterval = 1f;
        [SerializeField] private float lazyChaseStateDuration = 8f;
        [SerializeField] private float arenaWidth = 30f; // Placeholder - set this based on your arena

        [SerializeField] private Dictionary<DamageType, float> Damage = new Dictionary<DamageType, float>()
            { { DamageType.Physical, 15f } };
        
        // State references
        private BossInitState initState;
        private BossRidingState ridingState;
        private BossVulnerableState vulnerableState;
        private BossLazyChaseState lazyChaseState;
        private FSM.EnemyStates.DeadState deadState;

        // Boss state tracking
        private int currentRidingPass = 0;
        private const int MAX_RIDING_PASSES = 3;
        private float stateTimer = 0f;

        public int CurrentRidingPass => currentRidingPass;
        public float ArenaWidth => arenaWidth;
        public float RidingSpeed => ridingSpeed;
        public float LazyChaseSpeed => lazyChaseSpeed;
        public float LazyChaseDistance => lazyChaseDistance;
        public float StateTimer => stateTimer;

        public Vector3 spawnPoint;

        public override void Awake()
        {
            base.Awake();
            spawnPoint = this.transform.position;
        }

        public override void InitializeStateMachine()
        {
            stateMachine = new FSM.StateMachine();

            initState = new BossInitState(this);
            ridingState = new BossRidingState(this);
            vulnerableState = new BossVulnerableState(this);
            lazyChaseState = new BossLazyChaseState(this);
            deadState = new FSM.EnemyStates.DeadState(this);

            // Death transition from any state
            stateMachine.AddAnyTransition(deadState, new FuncPredicate(
                () => CurrentHealth <= 0
            ));

            // Init -> Riding (when target is set)
            stateMachine.AddTransition(initState, ridingState, new FuncPredicate(
                () => Target != null
            ));

            // Riding -> Vulnerable (after 3 passes)
            stateMachine.AddTransition(ridingState, vulnerableState, new FuncPredicate(
                () => currentRidingPass >= MAX_RIDING_PASSES
            ));

            // Vulnerable -> LazyChase (after duration)
            stateMachine.AddTransition(vulnerableState, lazyChaseState, new FuncPredicate(
                () => stateTimer >= vulnerableStateDuration
            ));

            // LazyChase -> Riding (after duration, cycle repeats)
            stateMachine.AddTransition(lazyChaseState, ridingState, new FuncPredicate(
                () => stateTimer >= lazyChaseStateDuration
            ));

            stateMachine.SetState(initState);
        }

        protected override void Update()
        {
            if (Target == null || !isInitialized) return;

            if (animator != null)
            {
                if (agent.desiredVelocity.sqrMagnitude > 0.01f)
                {
                    animator.SetBool("IsRunning", true);
                }
                else
                {
                    animator.SetBool("IsRunning", false);
                }
            }

            stateTimer += Time.deltaTime;
            stateMachine.Update();
        }

        public void IncrementRidingPass()
        {
            currentRidingPass++;
        }

        public void ResetRidingPass()
        {
            currentRidingPass = 0;
        }

        public void ResetStateTimer()
        {
            stateTimer = 0f;
        }

        public override void Action()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;

            var instance = PoolManager.Instance.projectilePools[bulletType].Get();
            var dirToPlayer = (Target.transform.position - this.transform.position).normalized;
            instance.transform.position = this.transform.position;
            instance.SetSpeed(10f);
            instance.Launch(dirToPlayer, Damage, LayerMask.GetMask("Damagable"));

            lastAttackTime = Time.time;
        }
        
        public void ShootWhileRiding()
        {
            var instance = PoolManager.Instance.projectilePools[bulletType].Get();
            var dirToPlayer = (Target.transform.position - this.transform.position).normalized;
            if (!IsPlayerCloseToCenter()) dirToPlayer.x = 0;
            instance.transform.position = this.transform.position;
            instance.SetSpeed(10f);
            instance.Launch(dirToPlayer, Damage, LayerMask.GetMask("Damagable"));

            lastAttackTime = Time.time;
        }

        private bool IsPlayerCloseToCenter()
        {
            Vector3 localPos = transform.InverseTransformPoint(Target.transform.position);
            return (Mathf.Abs(localPos.y) < 4f);
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