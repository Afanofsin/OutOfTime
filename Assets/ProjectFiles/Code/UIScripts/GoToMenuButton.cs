using ProjectFiles.Code.Events;
using UnityEngine;

namespace ProjectFiles.Code.UIScripts
{
    public class GoToMenuButton : MonoBehaviour
    {
        public void OnClick()
        {
            UIEvents.BackToMenu?.Invoke();
        }
        
    }
}