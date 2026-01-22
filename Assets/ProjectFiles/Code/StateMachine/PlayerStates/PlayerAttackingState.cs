using UnityEngine;

namespace FSM.PlayerStates
{
    public class PlayerAttackingState : BaseState
    {
        public bool IsComplete { get; private set; }
        private readonly System.Func<float> _getTime;
        private float _attackTime;
        private float _elapsedTime;
        private Player _player;
        public override void OnEnter()
        {
            _player.Attack();
            IsComplete = false;
            _elapsedTime = 0f;
            _attackTime = _getTime();
        }

        public override void OnUpdate()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _attackTime)
                IsComplete = true;
        }

        public override void OnFixedUpdate()
        {
            
        }

        public override void OnExit()
        {
            _elapsedTime = 0f;
            IsComplete = false;
        }
        
        public PlayerAttackingState(System.Func<float> timeProvider, Player player)
        {
            _getTime = timeProvider;
            _player = player;
        }
    }
}