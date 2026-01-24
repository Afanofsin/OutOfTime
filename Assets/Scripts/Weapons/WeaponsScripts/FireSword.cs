using UnityEngine;

public class FireSword : MeleeWeaponBase
{
    [SerializeField] private ParticleSystem particles;
    private ParticleSystem.ShapeModule _shape;
    
    private Vector3 _posOffset = new (1.5f, 0.35f, 0);
    private Vector3 _rotOffset = new (0f, 0f, 180f);
        
    private Vector3 _posOffsetNew = new (1.5f, -0.35f, 0);
    private Vector3 _rotOffsetNew = new (0f, 0f, 0);
    
     public override void Awake()
    {
        base.Awake();
        _shape = particles.shape;
    }
    
    public override void Update()
    {
        if (swing.IsRunning) return;
        
        weaponSprite.flipX = transform.localRotation.eulerAngles.z is > 90f and < 270f;
        if (weaponSprite.flipX)
        {
            _shape.position = _posOffset;
            _shape.rotation = _rotOffset;
        }
        else
        {
            _shape.position = _posOffsetNew;
            _shape.rotation = _rotOffsetNew;
        }
        weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 20f and < 160f ? 8 : 11;
    }
}
