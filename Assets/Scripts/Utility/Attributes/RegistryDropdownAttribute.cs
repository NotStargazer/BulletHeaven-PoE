using System;
using UnityEngine;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RegistryDropdownAttribute : PropertyAttribute
    {
        public Type RegistryType;
        
        public RegistryDropdownAttribute(Type type)
        {
            RegistryType = type;
        }
    }
}
