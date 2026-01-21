namespace FSM
{
    public class BaseState : IState
    {
        public virtual void OnEnter()
        { }

        public virtual void OnUpdate()
        { }

        public virtual void OnFixedUpdate()
        { }

        public virtual void OnExit()
        { }
    }
}