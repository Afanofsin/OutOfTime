using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using PrimeTween;


public class BloodThrustSwing : SwingBase, ISwing
{
    [SerializeField] private Vector3 startOffset;
    [SerializeField] private Vector3 endOffset;
    [SerializeField] private Ease ease;
    [SerializeField] private TrailRenderer trail;
    private Tween _swingTween;
    private IReadOnlyDictionary<DamageType, float> _damage;
    private readonly HashSet<Collider2D> _hitTargets = new();
    public bool IsRunning => _swingTween.isAlive;
    
    public override void Awake()
    {
        
    }

    public void OnEnable()
    {
        trail.emitting = false;
    }
    
    public void StartSwing(float attackAngle, IReadOnlyDictionary<DamageType, float> damage, float duration)
    {
        if (_swingTween.isAlive) return;
        
        _hitTargets.Clear();
        
        Vector2 direction = Quaternion.Euler(0f, 0f, attackAngle) * Vector2.right;

        Vector3 from = transform.localPosition;
        Vector3 to = from + (Vector3)(direction * endOffset.magnitude);

        transform.localRotation = Quaternion.Euler(0f, 0f, attackAngle);
        _damage = damage;
        
        trail.Clear();
        trail.emitting = true;
        _swingTween = Tween.Custom(
            from,
            to,
            duration,
            position =>
            {
                transform.localPosition = position;
            },
            ease
        ).OnComplete(() =>
        { 
            trail.emitting = false; 
            _hitTargets.Clear(); 
            Destroy(gameObject);
        }); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable) || damageable is Player)
            return;

        if (!_hitTargets.Add(other))
            return;
        
        damageable.TakeDamage(_damage);
        
    }
}
