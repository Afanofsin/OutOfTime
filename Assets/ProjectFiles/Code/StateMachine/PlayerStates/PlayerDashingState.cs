using PrimeTween;
using UnityEngine;

namespace FSM.PlayerStates
{
    public class PlayerDashingState : BaseState
    {
        public bool IsComplete { get; private set; }

        private readonly Rigidbody2D _targs;
        private readonly System.Func<Vector2> _getDirection;

        private float dashTime = 0.25f;
        private float _elapsedTime;
        private Vector2 _dashDir;

        public override void OnEnter()
        {
            IsComplete = false;
            _elapsedTime = 0f;
            _dashDir = _getDirection().normalized;
        }

        public override void OnUpdate()
        {
            _targs.linearVelocity = _dashDir * 20f;
            
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= dashTime)
                IsComplete = true;
        }

        public override void OnFixedUpdate()
        {
            
        }

        public override void OnExit()
        {
            _targs.linearVelocity = Vector2.zero;
            _elapsedTime = 0f;
            IsComplete = false;
        }

        public PlayerDashingState(Rigidbody2D targetRb, System.Func<Vector2> directionProvider)
        {
            _targs = targetRb;
            _getDirection = directionProvider;
        }
    }
}