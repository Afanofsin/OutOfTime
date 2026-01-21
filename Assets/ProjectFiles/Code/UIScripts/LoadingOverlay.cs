using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace ProjectFiles.Code.UIScripts
{
    public class LoadingOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TweenSettings<float> fadeInSettings;
        [SerializeField] private TweenSettings<float> fadeOutSettings;
        
        public async UniTask FadeIn()
        {
            await Tween.Alpha(canvasGroup, fadeInSettings);
        }
        
        public async UniTask FadeOut()
        {
            await Tween.Alpha(canvasGroup, fadeOutSettings);
        }
        
    }
}