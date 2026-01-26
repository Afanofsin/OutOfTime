using System;
using Cysharp.Threading.Tasks;
using ProjectFiles.Code.Events;
using ProjectFiles.Code.Other;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectFiles.Code.Controllers
{
    public class CoreManager : MonoBehaviour
    {
        public static CoreManager Instance { get; private set; }
        [SerializeField] private GameObject pauseMenu;

        public bool GoToMenu { get; set; } = false;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            GoToMenu = false;
        }

        private async void Start()
        {
            // TODO : Initialize Game here
            UIEvents.OnPause += PauseGame;
            UIEvents.OnResume += ResumeGame;
            UIEvents.BackToMenu += OpenMainMenu;

            
            
            await UniTask.WaitUntil(() => SceneController.Instance != null);
            await UniTask.WaitUntil(() => GameStateSystem.Instance != null);
            
        }

        private void PauseGame()
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        private void ResumeGame()
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }

        private void OpenMainMenu()
        {
            Time.timeScale = 1;
            GoToMenu = true;
            pauseMenu.SetActive(false);
        }
    }
}