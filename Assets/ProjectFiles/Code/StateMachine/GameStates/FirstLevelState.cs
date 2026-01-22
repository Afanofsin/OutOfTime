using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.Other;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FSM.GameStates
{
    public class FirstLevelState : BaseState
    {
        public bool MoveToPlay = false;
        public override async void OnEnter()
        {
            MoveToPlay = false;
            Debug.Log("FirstLevelLoading");
            await SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.FirstLevel)
                .Unload(SceneDatabase.Slots.MainMenu)
                .WithOverlay()
                .Perform();
            GameController.Instance.GenerateLevel();
            Debug.Log("$$$$FirstLevelLoaded$$$$");
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
            Debug.Log("Exiting First Level");
            MoveToPlay = false;
        }

    }
}