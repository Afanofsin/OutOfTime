using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using PrimeTween;

public abstract class SwingBase : MonoBehaviour
{
    protected Collider2D weaponCollider;
    protected readonly RaycastHit2D[] hits = new RaycastHit2D[10];
    
    public virtual void Awake() => weaponCollider = gameObject.GetComponent<BoxCollider2D>();
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
    public bool IsRunning => _swingTween.isAlive;
    
    private void Start() => trail.emitting = false;
    
    public void StartSwing(float attackAngle, IReadOnlyDictionary<DamageType, float> damage, float duration)
    {
        if (_swingTween.isAlive) return;
        
        _hitTargets.Clear();
        
        Vector2 direction = Quaternion.Euler(0f, 0f, attackAngle) * Vector2.right;

        Vector3 from = transform.localPosition;
        Vector3 to = from + (Vector3)(direction * endOffset.magnitude);

        transform.localRotation = Quaternion.Euler(0f, 0f, attackAngle);
        weaponCollider.enabled = true;
        _damage = damage;
        
        SoundManager.Instance.Play(SoundId.SwordAttack);
        trail.Clear();
        trail.emitting = true;
        _swingTween = Tween.Custom(
            from,
            to,
            duration * 0.5f,
            position =>
            {
                transform.localPosition = position;
            },
            ease
        ).OnComplete(() =>
        { 
            trail.emitting = false;
            _swingTween = Tween.Custom(
                to,
                from,
                duration * 0.5f,
                position =>
                {
                    transform.localPosition = position;
                },
                ease
            ).OnComplete(() =>
            { 
                weaponCollider.enabled = false;
                _hitTargets.Clear();
            }); 
            
        }); 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable) || damageable is Player)
            return;

        if (!_hitTargets.Add(other))
            return;

        Vector2 playerPos = PlayerController.Instance.GetPlayerPos();
        Vector2 targetPoint = other.ClosestPoint(other.transform.position);
        Vector2 direction = (targetPoint - playerPos).normalized;

        var size = Physics2D.RaycastNonAlloc(playerPos, direction, hits, Mathf.Infinity, weaponCollider.includeLayers);

        // Find the closest hit
        RaycastHit2D closestHit = default;
        float closestDistance = float.MaxValue;
        
        foreach (var hit in hits)
        {
            if (hit.collider == null)
                continue;

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestHit = hit;
            }
        }

        if (closestHit.collider == null)
            return;

        // If closest hit is the damageable target
        if (closestHit.collider == other)
        {
            damageable.TakeDamage(_damage);
        }
    }
}
