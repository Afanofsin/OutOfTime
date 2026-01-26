using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.Other;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace FSM.GameStates
{
    public class MainMenuState : BaseState
    {
        public bool MoveToPlay = false;
        private bool initial = false;
        public override void OnEnter()
        {
            CoreManager.Instance.GoToMenu = false;
            MoveToPlay = false;
                Debug.Log("Not Initial Menu");
                SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.MainMenu, SceneDatabase.Scenes.MainMenu)
                    .Unload(SceneDatabase.Slots.SessionContent)
                    .Unload(SceneDatabase.Slots.Session)
                    .WithOverlay()
                    .Perform();
        }

        public override void OnUpdate()
        {
            InputSystem.onAnyButtonPress.CallOnce( _ => MoveToPlay = true);
        }

        public override void OnFixedUpdate()
        {
            
        }

        public override void OnExit()
        {
            Debug.Log("Menu exit");
            MoveToPlay = false;
        }
    }
}