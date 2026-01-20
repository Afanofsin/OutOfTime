using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using PrimeTween;

public abstract class SwingBase : MonoBehaviour
{
    [SerializeField] private LayerMask hitMask;
    protected Collider2D weaponCollider;
    public virtual void Awake()
    {
        weaponCollider = gameObject.GetComponent<Collider2D>();
        weaponCollider.includeLayers = hitMask;
        weaponCollider.excludeLayers = 0;
    }
}

public class WeaponThrustSwing : SwingBase, ISwing
{
    [SerializeField] private Vector3 startOffset;
    [SerializeField] private Vector3 endOffset;
    [SerializeField] private Ease ease;
    [SerializeField] private TrailRenderer trail;
    private Tween _swingTween;
    private IReadOnlyDictionary<DamageType, float> _damage;
    private readonly HashSet<Collider2D> _hitTargets = new();
    
    public void StartSwing(float attackAngle, IReadOnlyDictionary<DamageType, float> damage, float duration)
    {
        
        if (_swingTween.isAlive) return;
        
        _hitTargets.Clear();
        
        Vector2 direction = Quaternion.Euler(0f, 0f, attackAngle) * Vector2.right;

        Vector3 from = transform.localPosition;
        Vector3 to   = from + (Vector3)(direction * endOffset.magnitude);

        transform.localRotation = Quaternion.Euler(0f, 0f, attackAngle);
        weaponCollider.enabled = true;
        _damage = damage;
        
        trail.Clear();
        _swingTween = Tween.Custom(
            from,
            to,
            duration,
            angle =>
            {
                transform.localPosition = angle;
            },
            ease
        ).OnComplete(() =>
        { 
            _swingTween = Tween.Custom(
                to,
                from,
                duration / 2.5f,
                angle =>
                {
                    transform.localPosition = angle;
                },
                ease
            ).OnComplete(() =>
            { 
                weaponCollider.enabled = false;
            }); 
            
        }); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_hitTargets.Add(other)) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
        }
        
        if (other.TryGetComponent<IAttackReactor>(out var reactor))
        {
            reactor.React();
        }
    }
}
