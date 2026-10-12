using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gameplay.Enemies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    public static class EditorStateMachineUtility
    {
        public static void CreateTypeSelector<T>(VisualElement root, SerializedProperty property, string label)
        {
            var typeNames = FlexibleStateMachine.StateType<T>.TypeNames.ToList();
            var nameOnly = new List<string>(typeNames.Count);
            foreach (var t in typeNames)
            {
                nameOnly.Add(t.Split('.')[^1]);
            }
            if (typeNames.Count == 0)
            {
                return;
            }
            
            property = property.FindPropertyRelative("_typeName");
            var index = 0;
            if (!string.IsNullOrEmpty(property.stringValue))
            {
                index = typeNames.IndexOf(property.stringValue);
            }
            var popup = new PopupField<string>(label, typeNames, index);
            popup.formatListItemCallback = s => ObjectNames.NicifyVariableName(nameOnly[typeNames.IndexOf(s)]);
            popup.formatSelectedValueCallback = s => ObjectNames.NicifyVariableName(nameOnly[typeNames.IndexOf(s)]);
            popup.BindProperty(property);
            root.Add(popup);
        }
        
        public static void ValidateParams(Type stateType, SerializedProperty paramsProperty)
        {
            var ctors = stateType.GetConstructors();
            var ctor = ctors[0];
            var prams = ctor.GetParameters();

            if (prams.Length != paramsProperty.arraySize)
            {
                SetParams(prams, paramsProperty);
                return;
            }

            for (var i = 0; i < prams.Length; i++)
            {
                var prop = paramsProperty.GetArrayElementAtIndex(i);
                var propType = prop.FindPropertyRelative("_type");
                var propName = prop.FindPropertyRelative("_name");
                var pramType = prams[i].ParameterType;
                if (pramType.FullName != propType.stringValue || pramType.Name != propName.stringValue)
                {
                    SetParams(prams, paramsProperty);
                    return;
                }
            }
        }

        private static void SetParams(ParameterInfo[] parameters, SerializedProperty paramsProperty)
        {
            if (parameters.Length == 0)
            {
                paramsProperty.arraySize = 0;
                return;
            }

            paramsProperty.arraySize = parameters.Length;

            for (var i = 0; i < paramsProperty.arraySize; i++)
            {
                var paramType = paramsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_type");
                var paramName = paramsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("_name");
                paramType.stringValue = parameters[i].ParameterType.FullName;
                paramName.stringValue = parameters[i].Name;
            }
        }
    }
}