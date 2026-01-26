using UnityEngine;

namespace Interfaces
{
    public interface IRangedEnemy : IEnemy
    {
        public float AttackRange { get; }
        public float RetreatDistance { get; }
        
        public void MoveTowardsTarget();
        public void MoveAwayFromTarget();
        public void StrafeAroundTarget();
        public float GetDistanceToTarget();
        public void FaceTarget();
        public void SetTarget(GameObject targetGo);
        public void OnDeath();
    }
}