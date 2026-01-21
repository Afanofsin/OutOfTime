using System;
using UnityEngine;

namespace ProjectFiles.Code.UIScripts
{
    public class UIContoller : MonoBehaviour
    {
        public static UIContoller Instance {get; private set;}

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        
    }
}