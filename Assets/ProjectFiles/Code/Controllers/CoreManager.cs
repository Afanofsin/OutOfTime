using System;
using Cysharp.Threading.Tasks;
using ProjectFiles.Code.Other;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectFiles.Code.Controllers
{
    public class CoreManager : MonoBehaviour
    {
        private async void Start()
        {
            // TODO : Initialize Game here
            
            await UniTask.WaitUntil(() => SceneController.Instance != null);
            await UniTask.WaitUntil(() => GameStateSystem.Instance != null);
            
        }

        private void Update()
        {

        }
    }
}