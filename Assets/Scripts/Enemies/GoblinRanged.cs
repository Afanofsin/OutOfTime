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
    public class GoblinRanged : RangedEnemyBase, IDamageable
    {
        private float dynamicAttackCooldown;
        
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
            dynamicAttackCooldown = attackCooldown;
        }

        public override void InitializeStateMachine()
        {
            stateMachine = new StateMachine();

            initState = new EnemyInitState(this);
            engageState = new EngageState(this);
            attackState = new GoblinRangeAttackState(this);
            retreatState = new GoblinRangeRetreatState(this);
            deadState = new DeadState(this);
            
            stateMachine.AddAnyTransition(deadState, new FuncPredicate(
                () => CurrentHealth <= 0
            ));
            
            stateMachine.AddTransition(initState, engageState, new FuncPredicate(() =>
                {
                    bool result = Target != null;
                    return result;
                }
            ));
            
            // Engage -> Attack (when in range)
            stateMachine.AddTransition(engageState, attackState, new FuncPredicate(
                () => GetDistanceToTarget() >= RetreatDistance && GetDistanceToTarget() <= AttackRange
            ));
        
            // Attack -> Retreat (when too close)
            stateMachine.AddTransition(attackState, retreatState, new FuncPredicate(
                () => GetDistanceToTarget() < RetreatDistance
                ));
        
            // Attack -> Engage (when too far)
            stateMachine.AddTransition(attackState, engageState, new FuncPredicate(
                () => GetDistanceToTarget() > AttackRange
            ));
        
            // Retreat -> Attack (when back in attack range)
            stateMachine.AddTransition(retreatState, attackState, new FuncPredicate(
                () => GetDistanceToTarget() >= RetreatDistance && GetDistanceToTarget() <= AttackRange
            ));
        
            // Retreat -> Engage (when far enough after retreating)
            stateMachine.AddTransition(retreatState, engageState, new FuncPredicate(
                () => GetDistanceToTarget() > AttackRange
            ));
            
            stateMachine.SetState(initState);
        }
        
        public override void Action()
        {
            Debug.Log("Goblin");
            dynamicAttackCooldown = attackCooldown * UnityEngine.Random.Range(1f, 1.8f);
            if (Time.time - lastAttackTime < dynamicAttackCooldown) return;
        
            var instance = PoolManager.Instance.projectilePools[bulletType].Get();
            var dirToPlayer = (Target.transform.position - this.transform.position).normalized;
            instance.transform.position = this.transform.position;
            instance.SetSpeed(8f);
            instance.Launch(dirToPlayer, Damage, LayerMask.GetMask("Damagable"));
        
            lastAttackTime = Time.time;
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