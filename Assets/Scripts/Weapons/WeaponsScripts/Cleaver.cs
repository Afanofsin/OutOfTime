using System;
using UnityEngine;

public class Cleaver : MeleeWeaponBase
{
    [SerializeField] private ParticleSystem particles;
    private ParticleSystem.ShapeModule _shape;
    
    private Vector3 _posOffset = new (3.75f, 1f, 0f);
        
    private Vector3 _posOffsetNew = new (3.75f, -1f, 0f);
    
     public override void Awake()
    {
        base.Awake();
        _shape = particles.shape;
        
    }

    public void Start()
    {
        particles.gameObject.SetActive(!interactCollider.enabled);
    }

    public override void Update()
    {
        if (swing.IsRunning) return;
        
        weaponSprite.flipX = transform.localRotation.eulerAngles.z is > 90f and < 270f;
        weaponSprite.sortingOrder = transform.localRotation.eulerAngles.z is > 0f and < 180 ? 8 : 11;
        
        if (weaponSprite.flipX)
        {
            _shape.position = _posOffset;
        }
        else
        {
            _shape.position = _posOffsetNew;
        }
    }
}
