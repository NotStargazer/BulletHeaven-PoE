using System.Collections.Generic;
using Gameplay.Enemies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FlexibleStateMachine))]
    public class FlexibleStateMachinePropertyDrawer : PropertyDrawer
    {
        public static List<string> States { get; private set; }
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var defaultState = property.FindPropertyRelative("_defaultState");
            var stateDefinitions = property.FindPropertyRelative("_stateDefinitions");
            
            States = new List<string>();
            for (var i = 0; i < stateDefinitions.arraySize; i++)
            { 
                var name = stateDefinitions.GetArrayElementAtIndex(i).FindPropertyRelative("_stateName");
                States.Add(name.stringValue);
            }
            
            var index = States.IndexOf(defaultState.stringValue);
            index = Mathf.Max(0, index);
            var popup = new PopupField<string>("Starting State", States, index);
            popup.BindProperty(defaultState);
            root.Add(popup);
            root.Add(new PropertyField(stateDefinitions));
            return root;
        }
    }
}