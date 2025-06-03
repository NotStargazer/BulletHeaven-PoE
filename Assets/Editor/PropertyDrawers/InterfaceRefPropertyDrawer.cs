using UnityEditor;
using UnityEngine;
using Utility;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(InterfaceRef<>))]
    public class InterfaceRefPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_target"), label);
        }
    }
}
