using System;
using UnityEngine;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RegistryDropdownAttribute : PropertyAttribute
    {
        public readonly Type RegistryType;
        public readonly string Filter;

        public RegistryDropdownAttribute(Type type, string filter = "")
        {
            RegistryType = type;
            Filter = filter;
        }
    }
}
