using System.Collections.Generic;
using System.Reflection;
using Gameplay.Enemies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FlexibleStateMachine.StateDefinition))]
    public class StateDefinitionPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var stateName = property.FindPropertyRelative("_stateName");
            var stateType = property.FindPropertyRelative("_stateType");
            var stateParams = property.FindPropertyRelative("_stateParameters");
            var exitConditions = property.FindPropertyRelative("_exitConditions");
            
            var assembly = Assembly.GetAssembly(typeof(IState));
            var typeName = stateType.FindPropertyRelative("_typeName");
            EditorStateMachineUtility.ValidateParams(assembly.GetType(typeName.stringValue), stateParams);
            root.Add(new PropertyField(stateName));
            root.Add(new PropertyField(stateType));
            for (var i = 0; i < stateParams.arraySize; i++)
            {
                var pram = stateParams.GetArrayElementAtIndex(i);
                root.Add(new PropertyField(pram));
            }
            root.Add(new PropertyField(exitConditions));
            return root;
        }
    }
}