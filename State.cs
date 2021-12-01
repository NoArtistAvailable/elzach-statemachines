using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace elZach.Common
{
    public class State : ScriptableObject
    {
        public event Action onEnter, onExit;
        public List<StateMachine.Link> links;

        public void Enter() => onEnter?.Invoke();
        public void Exit() => onExit?.Invoke();

        public void Clear()
        {
            onEnter = null;
            onExit = null;
        }
    }
    
#if UNITY_EDITOR

    [CustomEditor(typeof(State), true)]
    public class StateEditor : Editor
    {
        private StateMachine _parent;

        private StateMachine parent
        {
            get
            {
                if (!_parent)
                {
                    var t = target as State;
                    var path = AssetDatabase.GetAssetPath(t);
                    path.Remove(path.Length - t.name.Length, t.name.Length);
                    _parent = AssetDatabase.LoadAssetAtPath<StateMachine>(path);
                }

                return _parent;
            }
        }

        public override void OnInspectorGUI()
        {
            var t = target as State;
            EditorGUILayout.BeginHorizontal();
            string rename = EditorGUILayout.DelayedTextField("Name", t.name);
            if (GUILayout.Button("x", GUILayout.Width(18)))
            {
                string path = AssetDatabase.GetAssetPath(t);
                AssetDatabase.RemoveObjectFromAsset(t);
                AssetDatabase.ImportAsset(path);
            }

            EditorGUILayout.EndHorizontal();
            if (rename != t.name)
            {
                t.name = rename;
                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(parent);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(parent));
                AssetDatabase.Refresh();
            }

            DrawDefaultInspector();
        }
    }

#endif
}