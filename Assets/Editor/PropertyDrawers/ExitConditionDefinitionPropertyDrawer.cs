using System.Collections.Generic;
using System.Reflection;
using Gameplay.Enemies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FlexibleStateMachine.ExitConditionDefinition))]
    public class ExitConditionDefinitionPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var exitCondition = property.FindPropertyRelative("_exitCondition");
            var nextState = property.FindPropertyRelative("_nextState");
            var prams = property.FindPropertyRelative("_parameters");
            
            var assembly = Assembly.GetAssembly(typeof(IExitCondition));
            var typeName = exitCondition.FindPropertyRelative("_typeName");
            EditorStateMachineUtility.ValidateParams(assembly.GetType(typeName.stringValue), prams);
            root.Add(new PropertyField(exitCondition));
            var states = FlexibleStateMachinePropertyDrawer.States;
            var index = states.IndexOf(nextState.stringValue);
            index = Mathf.Max(0, index);
            var popup = new PopupField<string>("Next State", states, index);
            popup.BindProperty(nextState);
            root.Add(popup);
            for (var i = 0; i < prams.arraySize; i++)
            {
                var pram = prams.GetArrayElementAtIndex(i);
                root.Add(new PropertyField(pram));
            }

            return root;
        }
    }
}