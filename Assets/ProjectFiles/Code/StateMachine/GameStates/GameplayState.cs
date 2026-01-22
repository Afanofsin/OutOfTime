using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.Other;
using SuperTiled2Unity.Ase.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FSM.GameStates
{
    public class GameplayState : BaseState
    {
        private string sceneToLoad;
        private bool isFirst;
        public bool MoveToPlay = false;
        public override async void OnEnter()
        {
            if (isFirst)
            {
                Debug.Log("FirstLevelLoading");
                await SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                    .Load(SceneDatabase.Slots.SessionContent, sceneToLoad)
                    .Unload(SceneDatabase.Slots.MainMenu)
                    .WithOverlay()
                    .Perform();
                GameController.Instance.GenerateLevel();
                Debug.Log("FirstLevelLoaded");
            }
            else
            {
                Debug.Log("SecondLevelLoading");
                await SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.SessionContent, sceneToLoad)
                    .WithOverlay()
                    .Perform();
                GameController.Instance.GenerateLevel();
                Debug.Log("SecondLevelLoaded");
            }
            
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
            Debug.Log("Exiting Gameplay");
            MoveToPlay = false;
        }

        public GameplayState(string SceneToLoad, bool IsFirst)
        {
            isFirst = IsFirst;
            sceneToLoad = SceneToLoad;
        }
    }
}