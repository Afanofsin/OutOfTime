using PrimeTween;
using UnityEngine;

[ExecuteAlways]
public class AttackHandler : MonoBehaviour
{
   [SerializeField] private Vector2 weaponOffset;
   [SerializeField] private Vector2 weaponHitSize;
   [SerializeField] private BoxCollider2D weaponCollider;
   private Tween _swingTween;
   
   private void Start()
   {
      SetWeaponOffset(weaponOffset, weaponHitSize);
   }
   
   private void OnValidate()
   {
      SetWeaponOffset(weaponOffset, weaponHitSize);
   }

   private void SetWeaponOffset(Vector2 offset, Vector2 size)
   {
      weaponCollider.size = size;
      weaponCollider.offset = offset;
   }

   public void PerformAttack(float attackAngle)
   {
      weaponCollider.GetComponentInParent<WeaponSwing>().StartSwing(attackAngle);
   }
}
