using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace elZach.Common
{
    [CustomPropertyDrawer(typeof(State))]
    public class StateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //position = EditorGUI.PrefixLabel(position, label);
            if(EditorGUI.PropertyField(position, property, label))
                property.serializedObject.ApplyModifiedProperties();
        }
    }
}

