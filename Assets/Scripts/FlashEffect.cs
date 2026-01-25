using System;
using System.Collections;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    private SpriteRenderer _spriteRender;
    private readonly WaitForSeconds _time = new(0.1f);

    private void Awake() => _spriteRender = GetComponent<SpriteRenderer>();

    public void FlashEffects() => StartCoroutine(FLashRoutine());
    
    private IEnumerator FLashRoutine()
    {
        _spriteRender.material.SetInt("_Flash", 1);
        yield return _time;
        _spriteRender.material.SetInt("_Flash", 0);
    }
}
