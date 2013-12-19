using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeLogger.Lifecycle.Core
{
    public abstract class StateMachine<TStateIdentifier>
    {
        private readonly Dictionary<TStateIdentifier, State<TStateIdentifier>> _stateMappings;
        private readonly TStateIdentifier _defaultStateType;
        private State<TStateIdentifier> _currentState;

        protected StateMachine(IEnumerable<State<TStateIdentifier>> states, TStateIdentifier defaultStateType)
        {
            _defaultStateType = defaultStateType;
            _stateMappings = states.ToDictionary(state => state.StateDescriptor);
            _currentState = _stateMappings[_defaultStateType];
        }

        public void Transition(TStateIdentifier newStateType)
        {
            if(!_stateMappings.ContainsKey(newStateType))
                throw new InvalidOperationException(String.Format("Unsupported State: {0}", newStateType));

            if(!_currentState.AllowedFutureStates.Contains(newStateType))
                throw new InvalidOperationException(
                    String.Format("Cannot change state from {0} to {1}", _currentState.StateDescriptor, newStateType)
                );
            var newState = _stateMappings[newStateType];
            if(newState.Transition(_currentState.StateDescriptor))
                _currentState = newState;
        }

        public List<TStateIdentifier> TransitionableStates { get { return _currentState.AllowedFutureStates; } } 
    }
}