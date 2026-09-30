using UnityEditor;
using UnityEngine;
using Utility.Attributes;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(PercentageAttribute))]
    public class PercentagePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            if (property.type == "float")
            {
                position.x -= 15;
                position.width -= 220;
                property.floatValue = EditorGUI.FloatField(position, property.floatValue * 100) / 100;
                position.x += position.width - 14;
                EditorGUI.LabelField(position, "%");
            }
        }
    }
}