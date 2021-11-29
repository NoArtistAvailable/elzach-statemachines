using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace elZach.Common
{
    public class StateDirector : MonoBehaviour
    {
        public StateMachine stateMachine;
        public bool instantiate = true;
        public State[] states => stateMachine.states.ToArray();
        public StateMachine.StateEvents[] events;
        
        public Button<StateDirector> createEventsButton = new Button<StateDirector>(x => x.CreateEventListeners());

        internal Dictionary<State, State> instances = new Dictionary<State, State>();

        public State current => stateMachine.current;
        
        public void CreateEventListeners()
        {
            var eventListeners = new StateMachine.StateEvents[states.Length];
            for (int i = 0; i < eventListeners.Length; i++)
            {
                var existing = events?.FirstOrDefault(x => x.state == states[i]);
                eventListeners[i] = existing ?? new StateMachine.StateEvents(states[i]);
                eventListeners[i].name = states[i].name;
            }

            events = eventListeners;
        }

        public void Start()
        {
            Init();
        }

        public void Init()
        {
            if (instantiate)
            {
                var originalStateMachine = stateMachine;
                stateMachine = Instantiate(stateMachine);
                originalStateMachine.name = name;
                for (int i = 0; i < stateMachine.states.Count; i++)
                {
                    var state = stateMachine.states[i];
                    stateMachine.states[i] = Instantiate(state);
                    stateMachine.states[i].name = state.name;
                    if (instances.ContainsValue(state))
                    {
                        // instances.TryGetValue(state, out var originalState);
                        // instances[originalState] = stateMachine.states[i];
                    }
                    else instances.Add(state, stateMachine.states[i]);
                }
            }
            stateMachine.Init(this);
        }

        public void Enter(State state)
        {
            //Debug.Log($"Entering state {state.name}");
            var targetState = state;
            //we expect a state which was created from the scriptable object original
            if (instances.ContainsKey(state)) targetState = instances[state];
            if (!stateMachine.states.Contains(targetState))
            {
                Debug.LogError($"State {targetState.name} not in state machine!", this);
                return;
            }
            //Debug.Log("Setting state");
            stateMachine.ForceState(targetState);
        }

        public void Enter(string value) => Enter(stateMachine.states.FirstOrDefault(x => x.name == value));
    }
}

