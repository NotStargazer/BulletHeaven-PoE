using System;
using System.Reflection;
using Gameplay.Data;
using UnityEditor;
using UnityEngine;
using Utility.Attributes;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(StatType))]
    public class StatSelectorPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            return (EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing) * 3;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var baseTypeProp = property.FindPropertyRelative("_baseType");
            var subTypeProp = property.FindPropertyRelative("_subType");

            var fields = typeof(BaseType).GetFields();
            var field = fields[baseTypeProp.enumValueIndex + 1];
            var subType = ((SubTypeAttribute)Attribute.GetCustomAttribute(field, typeof(SubTypeAttribute))).SubType;
            var subEnumValues = Enum.GetValues(subType);
            var height = EditorGUI.GetPropertyHeight(baseTypeProp) + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.BeginProperty(position, label, property);
            var labelRect = new Rect(position.x, position.y, position.width, height);
            var baseRect = new Rect(position.x, position.y + height, position.width, height);
            var subRect = new Rect(position.x, position.y + height * 2, position.width, height);
            
            EditorGUI.LabelField(labelRect, "Stat Type", new GUIStyle("BoldLabel"));
            baseTypeProp.enumValueIndex = (int)Convert.ChangeType(EditorGUI.EnumPopup(baseRect, (BaseType)baseTypeProp.enumValueIndex), typeof(BaseType));
            subTypeProp.intValue = Mathf.Clamp(subTypeProp.intValue, 0, subEnumValues.Length - 1);
            var sv = (Enum)subEnumValues.GetValue(subTypeProp.intValue);
            subTypeProp.intValue = (int)Convert.ChangeType(EditorGUI.EnumPopup(subRect, sv), subType);
            EditorGUI.EndProperty();
        }
    }
    

}
