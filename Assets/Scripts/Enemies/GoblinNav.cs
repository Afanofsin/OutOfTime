using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class GoblinNav : EnemyEntityBase, IDamageable
    {
        [SerializeField] NavMeshAgent _navMeshAgent;
        bool TargetSet = false;
        
        public void TakeDamage(IReadOnlyDictionary<DamageType, float> damage)
        {
            foreach (var damageKvp in damage)
            {
                CurrentHealth -= Mathf.Max(0, damageKvp.Value - damageKvp.Value * (resists[damageKvp.Key] / 100));
            }
            React();
        }

        public override void Awake()
        {
            base.Awake();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
        }

        private void Start()
        {
            // StartCoroutine(DebugAgent());
        }

        private IEnumerator DebugAgent()
        {
            yield return new WaitForSeconds(0.5f);
        
            Debug.Log("=== AGENT DEBUG ===");
            Debug.Log($"Position: {transform.position}");
            Debug.Log($"Is on NavMesh: {_navMeshAgent.isOnNavMesh}");
            Debug.Log($"Is enabled: {_navMeshAgent.enabled}");
            Debug.Log($"Speed: {_navMeshAgent.speed}");
            Debug.Log($"IsStopped: {_navMeshAgent.isStopped}");
            Debug.Log($"Has path: {_navMeshAgent.hasPath}");
            Debug.Log($"Path status: {_navMeshAgent.pathStatus}");
            Debug.Log($"Remaining distance: {_navMeshAgent.remainingDistance}");
            Debug.Log($"Velocity: {_navMeshAgent.velocity}");
            Debug.Log($"Desired velocity: {_navMeshAgent.desiredVelocity}");
        
            if (Target != null)
            {
                Debug.Log($"Target position: {Target.transform.position}");
                Debug.Log($"Distance to target: {Vector3.Distance(transform.position, Target.transform.position)}");
            }
            else
            {
                Debug.Log("Target is NULL!");
            }
        }
        
        public void Update()
        {
            if (!_navMeshAgent.isOnNavMesh)
            {
                Debug.LogWarning("Agent not on NavMesh!");
                return;
            }
        
            if (!TargetSet || Target == null)
            {
                return;
            }
        
            // Only set destination if we have a valid path or it changed significantly
            if (!_navMeshAgent.hasPath || 
                Vector3.Distance(_navMeshAgent.destination, Target.transform.position) > 0.5f)
            {
                _navMeshAgent.SetDestination(Target.transform.position);
            }
        }

        public override void SetTarget(GameObject targetGo)
        {
            base.SetTarget(targetGo);
            TargetSet = true;
            _navMeshAgent.SetDestination(targetGo.transform.position);
        }

        public override void React()
        {
        
        }
        public override void Action()
        {
        
        }
    
        public override void Heal(float amount)
        {
        
        }
    }
}