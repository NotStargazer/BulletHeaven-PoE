using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility.Attributes;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(RegistryDropdownAttribute))]
    public class RegistryDropdownPropertyDrawer : PropertyDrawer
    {
        private int? _popupValue;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var regAtt = attribute as RegistryDropdownAttribute;
            var fieldNames = regAtt!.RegistryType.GetFields().Select(field => field.Name).ToList();
            _popupValue ??= fieldNames.IndexOf(property.stringValue);
            _popupValue = EditorGUI.Popup(position, property.displayName, _popupValue.Value, fieldNames.ToArray());
            if (_popupValue >= 0)
            {
                property.stringValue = fieldNames[_popupValue.Value];
            }
        }
    }
}
