using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.Other;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace FSM.GameStates
{
    public class InitialMainMenu  : BaseState
    {
        public bool MoveToPlay = false;
        private bool initial = false;
        public override void OnEnter()
        {
            CoreManager.Instance.GoToMenu = false;
            MoveToPlay = false;
            Debug.Log("Initial Menu");
            SoundManager.Instance.PlayMainMenuMusic();
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