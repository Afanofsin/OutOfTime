using System;
using ProjectFiles.Code.Other;
using UnityEngine;

namespace ProjectFiles.Code.Controllers
{
    public class CoreManager : MonoBehaviour
    {
        private void Start()
        {
            // TODO : Initialize Game here

            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.MainMenu, SceneDatabase.Scenes.MainMenu)
                .WithOverlay()
                .Perform();
        }
    }
}