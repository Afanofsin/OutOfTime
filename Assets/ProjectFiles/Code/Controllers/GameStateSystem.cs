using UnityEngine;
using FSM;
using FSM.GameStates;

namespace ProjectFiles.Code.Controllers
{
    public class GameStateSystem : MonoBehaviour
    {
        public static GameStateSystem Instance { get; private set; }
        
        private IState CurrentState => stateMachine.GetState();
        private FSM.StateMachine stateMachine;
        
        private MainMenuState mainMenuState;
        private MainMenuState initialMenuState;
        private FirstLevelState firstLevelState;
        private SecondLevelState secondLevelState;
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
            stateMachine = new FSM.StateMachine();
            initialMenuState = new MainMenuState();
            mainMenuState = new MainMenuState();
            firstLevelState = new FirstLevelState();
            secondLevelState = new SecondLevelState();
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

            stateMachine.AddAnyTransition(mainMenuState, new FuncPredicate(
                () => CoreManager.Instance.GoToMenu
            ));
            
            stateMachine.SetState(initialState);
        }

        private void Update()
        {
            stateMachine?.Update();
        }

        private void FixedUpdate()
        {
            stateMachine?.FixedUpdate();
        }
    }
}