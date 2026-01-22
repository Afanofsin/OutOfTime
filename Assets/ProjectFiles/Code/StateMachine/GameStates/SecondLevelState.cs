using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.Other;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FSM.GameStates
{
    public class SecondLevelState : BaseState
    {
        public bool MoveToPlay = false;
        public override async void OnEnter()
        {
            MoveToPlay = false;
            Debug.Log("$$$$SecondLevelLoading$$$$");
            await SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.SecondLevel)
                .WithOverlay()
                .Perform();
            GameController.Instance.GenerateLevel();
            Debug.Log("SecondLevelLoaded");
        }

        public override void OnUpdate()
        {
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                MoveToPlay = true;
            }
        }

        public override void OnFixedUpdate()
        {
            
        }

        public override void OnExit()
        {
            Debug.Log("Exiting SecondLevel");
            MoveToPlay = false;
        }

    }
}