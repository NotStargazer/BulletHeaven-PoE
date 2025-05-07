using System;

namespace Utility.Attributes
{
    public class SubTypeAttribute : Attribute
    {
        public Type SubType;

        public SubTypeAttribute(Type subType)
        {
            SubType = subType;
        }
    }
}
