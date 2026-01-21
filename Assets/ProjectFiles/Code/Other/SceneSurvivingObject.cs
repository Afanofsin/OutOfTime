using System;
using UnityEngine;

namespace ProjectFiles.Code.Other
{
    public class SceneSurvivingObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}