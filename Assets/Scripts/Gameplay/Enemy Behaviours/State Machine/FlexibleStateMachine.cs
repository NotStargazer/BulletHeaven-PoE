using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Enemies
{
    [Serializable]
    public partial class FlexibleStateMachine
    {
        [SerializeField] private string _defaultState;
        [SerializeField] private StateDefinition[] _stateDefinitions;

        private FlexibleState _currentState;
        private Dictionary<string, FlexibleState> _states;
        
        public void Construct(Enemy subject)
        {
            _states = new Dictionary<string, FlexibleState>();
            foreach (var stateDefinition in _stateDefinitions)
            {
                stateDefinition.AddTo(_states, subject);
            }
            ChangeState(_defaultState);
        }

        public void Update()
        {
            _currentState.State?.Update();
            foreach (var exitCondition in _currentState.ExitConditions)
            {
                if (exitCondition.ExitCon.ShouldExit())
                {
                    ChangeState(exitCondition.NextState);
                }
            }
        }

        public void ChangeState(string state)
        {
            if (!_states.TryGetValue(state, out var newState))
            {
                Debug.LogError($"State {state} does not exist");
                return;
            }
            
            _currentState.State?.Exit();
            _currentState = newState;
            _currentState.State.Enter();
        }
    }
}