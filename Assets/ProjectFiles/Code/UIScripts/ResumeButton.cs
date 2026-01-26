using ProjectFiles.Code.Events;
using UnityEngine;

namespace ProjectFiles.Code.UIScripts
{
    public class ResumeButton : MonoBehaviour
    {
        public void OnClick()
        {
            UIEvents.OnResume?.Invoke();
        }
    }
}