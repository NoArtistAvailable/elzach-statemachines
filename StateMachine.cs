using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace elZach.Common
{
    [CreateAssetMenu(menuName = "StateMachine")]
    public class StateMachine : ScriptableObject
    {
        [System.Serializable]
        public class StateEvents
        {
            [HideInInspector] public string name;
            [HideInInspector] public State state;
            public UnityEvent OnEnter = new UnityEvent(), OnExit = new UnityEvent();

            public StateEvents(State state)
            {
                this.state = state;
                this.name = state.name;
            }
        }
        
        [System.Serializable]
        public class Link
        {
            public State target;
            public Func<bool> condition = () => true;
        }

        public List<State> states;
        public UnityEvent<State> OnStateEnter;
        public UnityEvent<State> OnStateExit;
        #region runtime
        [NonSerialized] public State current;
        #endregion
        
        public void Init(StateDirector director)
        {
            foreach (var state in states)
            {
                state.Clear();
                state.onEnter += ()=>OnStateEnter.Invoke(state);
                state.onExit += ()=>OnStateEnter.Invoke(state);

                var listener = director.events.FirstOrDefault(x =>
                    (director.instantiate ? director.instances[x.state] : x.state) == state);
                if (listener!=null)
                {
                    state.onEnter += listener.OnEnter.Invoke;
                    state.onExit += listener.OnExit.Invoke;
                }
            }
            ForceState(states[0],false);
        }

        public bool EnterState(State state)
        {
            var toCheck = current;
            var link = toCheck.links.FirstOrDefault(x => x.target == state);
            if (link == null || !link.condition.Invoke()) return false;
            ForceState(state, true);
            return true;
        }

        public void ForceState(State state, bool exitPrevious = true)
        {
            if (current != null && current != state) current.Exit();
            current = state;
            current.Enter();
        }
        
#if UNITY_EDITOR
        public Button<StateMachine> addStateButton = new Button<StateMachine>(x => x.AddNewState());
        public void AddNewState()
        {
            var state = CreateInstance<State>();
            if (states == null) states = new List<State>();
            states.Add(state);
            AssetDatabase.AddObjectToAsset(state, this);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
            AssetDatabase.Refresh();
        }
#endif
    }
}
