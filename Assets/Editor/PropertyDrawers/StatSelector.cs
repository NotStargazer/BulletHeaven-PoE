using System;
using System.Linq;
using System.Reflection;
using Gameplay.Data;
using UnityEditor;
using UnityEngine.UIElements;
using Utility.Attributes;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(StatType))]
    public class StatSelector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var baseType = property.FindPropertyRelative("_baseType");
            var baseEnumValues = Enum.GetValues(typeof(BaseType));
            var memberInfo = typeof(BaseType).GetCustomAttributes<SubTypeAttribute>(false).ToArray();
            var subType = memberInfo[baseType.enumValueIndex].SubType;
            var subEnumValues = Enum.GetNames(subType).ToList();
            
            var container = new VisualElement();
            var subTypeDropdown = new field(subEnumValues, 0, i =>
            {
                return Enum.Get;
            })
            container.Add(new EnumField(BaseType.Damage));
        }
    }
    

}
