using UnityEngine;
using FSM;
using FSM.GameStates;

namespace ProjectFiles.Code.Controllers
{
    public class GameStateSystem : MonoBehaviour
    {
        public static GameStateSystem Instance { get; private set; }
        
        private IState CurrentState => stateMachine.GetState();
        private StateMachine stateMachine;
        
        private MainMenuState mainMenuState;
        private LoadLevelState loadLevelState;
        private GameplayState gameplayState;
        private PausedState pausedState;
        private TutorialState tutorialState;
        private InitialState initialState;
        
        private void InitializeStateMachine()
        {
            stateMachine = new StateMachine();

            mainMenuState = new MainMenuState();
            loadLevelState = new LoadLevelState();
            gameplayState = new GameplayState();
            pausedState = new PausedState();
            tutorialState = new TutorialState();
            initialState = new  InitialState();
            
            // Transitions
            stateMachine.AddTransition(initialState, mainMenuState, new FuncPredicate(
                () => true
            ));
            
            stateMachine.AddTransition(mainMenuState, gameplayState, new FuncPredicate(
                () => true
            ));
            
            stateMachine.AddAnyTransition(mainMenuState, new FuncPredicate(
                () => true
            ));
            
            stateMachine.SetState(initialState);
        }
        
    }
}