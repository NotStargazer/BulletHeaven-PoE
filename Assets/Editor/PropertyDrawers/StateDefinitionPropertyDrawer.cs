using System;
using System.Linq;
using Gameplay.Enemies;
using UnityEditor;
using UnityEngine;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FlexibleStateMachine.StateDefinition))]
    public class StateDefinitionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var stateType = property.FindPropertyRelative("_stateType");
            var stateParams = property.FindPropertyRelative("_stateParameters");
            var exitConditions = property.FindPropertyRelative("_exitConditions");
            var exitConditionParams = property.FindPropertyRelative("_exitConditionParameters");

            EditorGUI.BeginProperty(position, label, property);
            position = DoStateType<IState>(position, stateType, new GUIContent("State Type"));
            
            EditorGUI.EndProperty();
        }

        private Rect DoStateType<T>(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeNames = FlexibleStateMachine.StateType<T>.TypeNames.ToArray();
            var afterLabel = EditorGUI.PrefixLabel(position, label);
            position.y += EditorGUIUtility.singleLineHeight;
            
            if (typeNames.Length == 0)
            {
                return position;
            }
            
            property = property.FindPropertyRelative("_typeName");
            var index = 0;
            if (!string.IsNullOrEmpty(property.stringValue))
            {
                index = Array.FindIndex(typeNames, t => t == property.stringValue);
            }
            var outIndex = EditorGUI.Popup(afterLabel, index, typeNames);
            property.stringValue = typeNames[outIndex];
            return position;
        }
    }
}