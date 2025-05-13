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
    
    [AttributeUsage(AttributeTargets.Field)]
    public class DisplayNameOverrideAttribute : Attribute
    {
        public string DisplayName;

        public DisplayNameOverrideAttribute(string displayName)
        {
            DisplayName = displayName;
        }
        
        public DisplayNameOverrideAttribute()
        {
            DisplayName = string.Empty;
        }
    }
}
