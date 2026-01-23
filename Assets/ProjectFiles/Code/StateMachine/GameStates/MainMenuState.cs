using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.Other;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FSM.GameStates
{
    public class MainMenuState : BaseState
    {
        public bool MoveToPlay = false;
        private bool initial = false;
        public override void OnEnter()
        {
            MoveToPlay = false;
            if (!initial)
            {
                Debug.Log("Not Initial Menu");
                SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.MainMenu, SceneDatabase.Scenes.MainMenu)
                    .Unload(SceneDatabase.Slots.SessionContent)
                    .Unload(SceneDatabase.Slots.Session)
                    .WithOverlay()
                    .Perform();
            }
            Debug.Log("Initial Menu");
        }

        public override void OnUpdate()
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                MoveToPlay = true;
            }
        }

        public override void OnFixedUpdate()
        {
            
        }

        public override void OnExit()
        {
            Debug.Log("Menu exit");
            MoveToPlay = false;
        }

        public MainMenuState(bool Initial)
        {
            initial = Initial;
        }
    }
}