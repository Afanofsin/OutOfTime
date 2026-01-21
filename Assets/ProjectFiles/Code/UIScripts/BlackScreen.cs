using System;
using Cysharp.Threading.Tasks;
using EasyTextEffects;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace ProjectFiles.Code.UIScripts
{
    public class BlackScreen : MonoBehaviour
    {
        [SerializeField] private TextEffect textEffect;
        [SerializeField] TweenSettings<float> tweenSettings;
        private Image image;
            
        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void Start()
        {
            InputSystem.onAnyButtonPress.CallOnce( _ => DeleteBlackScreen());
        }

        public void DisableText()
        {
            textEffect.gameObject.SetActive(false);
        }
        
        private void DeleteBlackScreen()
        {
            textEffect.StopOnStartEffects();
            textEffect.StartManualEffects();
            Tween.Alpha(image, tweenSettings)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}