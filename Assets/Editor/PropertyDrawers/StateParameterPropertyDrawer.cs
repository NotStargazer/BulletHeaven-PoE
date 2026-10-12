using System;
using Gameplay.Enemies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FlexibleStateMachine.StateParameter))]
    public class StateParameterPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var type = property.FindPropertyRelative("_type").stringValue;
            var name = property.FindPropertyRelative("_name").stringValue;
            name = ObjectNames.NicifyVariableName(name);
            SerializedProperty prop = null;
            if (Type.GetType(type) == typeof(string))
                prop = property.FindPropertyRelative("_stringVal");
            if (Type.GetType(type) == typeof(bool))
                prop = property.FindPropertyRelative("_boolVal");
            if (Type.GetType(type) == typeof(int))
                prop = property.FindPropertyRelative("_intVal");
            if (Type.GetType(type) == typeof(float))
                prop = property.FindPropertyRelative("_floatVal");
            if (Type.GetType(type) == typeof(Vector2))
                prop = property.FindPropertyRelative("_vector2Val");
            if (Type.GetType(type) == typeof(Vector3))
                prop = property.FindPropertyRelative("_vector3Val");
            if (Type.GetType(type) == typeof(Vector2Int))
                prop = property.FindPropertyRelative("_vector2IntVal");
            if (Type.GetType(type) == typeof(Vector3Int))
                prop = property.FindPropertyRelative("_vector3IntVal");
            if (Type.GetType(type) == typeof(Quaternion))
                prop = property.FindPropertyRelative("_quaternionVal");
            if (prop == null) return root;
            root.Add(new PropertyField(prop, name));
            return root;
        }
    }
}