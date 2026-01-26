using System;
using System.Collections.Generic;
using PrimeTween;

namespace FSM
{
    public class StateMachine
    {
        StateNode current;
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();

        public void Update()
        {
            var transition = GetTransition();
            if(transition != null)
                ChangeState(transition.ToState);

            current.State?.OnUpdate();
        }
        
        public void FixedUpdate()
        {
            current.State?.OnFixedUpdate();
        }
        
        // Setting state without triggering OnEnter
        // Initiation only
        public void SetState(IState state)
        {
            current = nodes[state.GetType()];
        }
        
        void ChangeState(IState state)
        {
            if (state == current.State) return;

            var previousState = current.State;
            var nextState = nodes[state.GetType()].State;

            previousState?.OnExit();
            nextState.OnEnter();
            current = nodes[state.GetType()];
        }

        ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
            {
                if(transition.Condition.Evaluate())
                    return transition;
            }

            foreach (var transition in current.Transitions)
            {
                if(transition.Condition.Evaluate())
                    return transition;
            }

            return null;
        }

        public void AddTransition(IState fromState, IState toState, IPredicate condition)
        {
            GetOrAddNode(fromState).AddTransition(GetOrAddNode(toState).State, condition);
        }

        public void AddAnyTransition(IState toState, IPredicate condition)
        {
            GetOrAddNode(toState);
            anyTransitions.Add(new Transition(toState, condition));
        }

        StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());

            if(node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;
        }
        
        public IState GetState() => current.State;

        private class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState state, IPredicate condition) => 
                Transitions.Add(new Transition(state, condition));
            
        }
    } 
}
