using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gamelogic.FSM
{
    public struct FsmStateChangedArguments<TStateEnum>
    {
        public TStateEnum OldState;
        public TStateEnum NewState;
    }
    public delegate void FsmStateChanged<TStateEnum>(FiniteStateMachine<TStateEnum> sender, FsmStateChangedArguments<TStateEnum> args);

    public abstract class FiniteStateMachine<TStateEnum>
    {
        public TStateEnum CurrentState { get; private set; }

        private IDictionary<TStateEnum, IList<TStateEnum>> transitions;
        private IDictionary<TStateEnum, IFsmState> states;

        public event FsmStateChanged<TStateEnum> StateChanged;

        protected void SetStates(IDictionary<TStateEnum, IFsmState> inStates)
        {
            states = inStates;
        }

        protected void SetTransitions(IDictionary<TStateEnum, IList<TStateEnum>> inTransitions)
        {
            transitions = inTransitions;
        }

        public void Tick()
        {
            states[CurrentState].Tick();
        }

        public void OnEnable(TStateEnum initialState)
        {
            OnEnableImpl();
            CurrentState = initialState;
            states[CurrentState].Enter();          
        }

        public void OnDisable()
        {
            states[CurrentState].Exit(true);
            OnDisableImpl();
        }

        protected virtual void OnEnableImpl() { }

        protected virtual void OnDisableImpl() { }

        public void TransitionTo(TStateEnum nextState)
        {
            if (IsValidTransition(nextState))
            {
                TStateEnum previousState = CurrentState;

                states[CurrentState].Exit(false);
                CurrentState = nextState;
                states[CurrentState].Enter();

                OnStateChange(previousState, nextState);
            }
            else
            {
                Debug.LogErrorFormat("Invalid transition from {0} to {1} detected.", CurrentState, nextState);
            }
        }

        public void OnStateChange(TStateEnum oldState, TStateEnum newState)
        {
            if (StateChanged != null)
            {
                StateChanged(this, new FsmStateChangedArguments<TStateEnum> {OldState = oldState, NewState = newState});
            }
        }

        public bool IsValidTransition(TStateEnum nextState)
        {
            return transitions[CurrentState].Contains(nextState);
        }
    }
}
