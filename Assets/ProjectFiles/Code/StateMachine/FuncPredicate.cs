using System;

namespace FSM
{
    public class FuncPredicate : IPredicate
    {
        private Func<bool> function;
        
        public bool Evaluate() => function.Invoke();

        public FuncPredicate(Func<bool> function)
        {
            this.function = function;
        }
    }
}