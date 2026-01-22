using System;
using UnityEngine;
using FSM;
using FSM.GameStates;
using ProjectFiles.Code.Other;

namespace ProjectFiles.Code.Controllers
{
    public class GameStateSystem : MonoBehaviour
    {
        public static GameStateSystem Instance { get; private set; }
        
        private IState CurrentState => stateMachine.GetState();
        private StateMachine stateMachine;
        
        private MainMenuState mainMenuState;
        private MainMenuState initialMenuState;
        private LoadLevelState loadLevelState;
        private GameplayState firstLevelState;
        private GameplayState secondLevelState;
        private PausedState pausedState;
        private TutorialState tutorialState;
        private InitialState initialState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            stateMachine = new StateMachine();

            initialMenuState = new MainMenuState(true);
            mainMenuState = new MainMenuState(false);
            loadLevelState = new LoadLevelState();
            firstLevelState = new GameplayState(SceneDatabase.Scenes.FirstLevel, true);
            secondLevelState = new GameplayState(SceneDatabase.Scenes.SecondLevel, false);
            pausedState = new PausedState();
            tutorialState = new TutorialState();
            initialState = new  InitialState();
            
            // Transitions
            stateMachine.AddTransition(initialState, initialMenuState, new FuncPredicate(
                () => true
            ));
            
            stateMachine.AddTransition(initialMenuState, firstLevelState, new FuncPredicate(
                () => initialMenuState.MoveToPlay
            ));
            
            stateMachine.AddTransition(mainMenuState, firstLevelState, new FuncPredicate(
                () => mainMenuState.MoveToPlay
            ));
            
            stateMachine.AddTransition(firstLevelState, secondLevelState, new FuncPredicate(
                () => firstLevelState.MoveToPlay
            ));
            
            stateMachine.AddTransition(secondLevelState, mainMenuState, new FuncPredicate(
                () => secondLevelState.MoveToPlay
            ));
            
            /*stateMachine.AddAnyTransition(mainMenuState, new FuncPredicate(
                () => true
            ));*/
            
            stateMachine.SetState(initialState);
        }

        private void Update()
        {
            Debug.LogWarning(CurrentState);
            stateMachine?.Update();
        }

        private void FixedUpdate()
        {
            stateMachine?.FixedUpdate();
        }
    }
}