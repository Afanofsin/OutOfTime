using ProjectFiles.Code.Controllers;
using ProjectFiles.Code.Other;
using UnityEngine;

namespace FSM.GameStates
{
    public class InitialState : BaseState
    {
        public override void OnExit()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.MainMenu, SceneDatabase.Scenes.MainMenu)
                .Perform();
        }
    }
}