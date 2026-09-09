using System;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SubTypeAttribute : Attribute
    {
        public Type SubType;

        public SubTypeAttribute(Type subType)
        {
            SubType = subType;
        }
    }
}
