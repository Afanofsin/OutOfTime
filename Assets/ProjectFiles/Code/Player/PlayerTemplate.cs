using FSM;
using FSM.PlayerStates;
using UnityEngine;

namespace ProjectFiles.Code.Player
{
    public class PlayerTemplate : MonoBehaviour
    {
        public IState CurrentState;
        private StateMachine stateMachine;
        
        private PlayerIdleState idleState;
        private PlayerMovingState movingState;
        private PlayerAttackingState attackingState;
        private PlayerInteractingState interactingState;
        private PlayerDashingState dashingState;

        private void InitializeStateMachine()
        {
            stateMachine = new StateMachine();
            
            idleState = new PlayerIdleState();
            movingState = new PlayerMovingState();
            attackingState = new PlayerAttackingState();
            interactingState = new PlayerInteractingState();
            dashingState = new PlayerDashingState();
            
            // Transitions
            stateMachine.AddTransition(idleState, movingState, new FuncPredicate(
                () => true
            ));
            
            stateMachine.AddAnyTransition(interactingState, new FuncPredicate(
                () => true
            ));
            
            stateMachine.SetState(idleState);
            CurrentState = stateMachine.GetState();
        }
    }
}