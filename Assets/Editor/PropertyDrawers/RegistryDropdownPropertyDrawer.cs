using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Utility.Attributes;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(RegistryDropdownAttribute))]
    public class RegistryDropdownPropertyDrawer : PropertyDrawer
    {
        private List<FieldInfo> _filtered = new();
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var regAtt = attribute as RegistryDropdownAttribute;
            var fields = regAtt!.RegistryType.GetFields();
            
            if (!string.IsNullOrEmpty(regAtt.Filter))
            {
                foreach (var field in fields)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(RegistryFilterAttribute)) is RegistryFilterAttribute filter
                        && regAtt.Filter == filter.Filter)
                    {
                        _filtered.Add(field);
                    }
                }

                fields = _filtered.ToArray();
                _filtered.Clear();
            }
            
            var fieldNames = fields.Select(field => field.Name).ToList();
            var popupValue = fieldNames.IndexOf(property.stringValue);
            popupValue = EditorGUI.Popup(position, property.displayName, popupValue, fieldNames.ToArray());
            if (popupValue >= 0)
            {
                property.stringValue = fieldNames[popupValue];
            }
        }
    }
}
