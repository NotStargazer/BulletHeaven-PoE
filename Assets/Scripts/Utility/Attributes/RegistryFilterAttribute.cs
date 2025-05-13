using System;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RegistryFilterAttribute : Attribute
    {
        public string Filter;

        public RegistryFilterAttribute(string filter)
        {
            Filter = filter;
        }
    }
}
