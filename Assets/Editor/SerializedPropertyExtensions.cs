using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Editor
{
    public static class SerializedPropertyExtensions
    {
        public static IEnumerable<SerializedProperty> GetDirectChildren(this SerializedProperty parent)
        {
            var dots = parent.propertyPath.Count(c => c == '.');
            foreach (SerializedProperty inner in parent)
            {
                var isDirectChild = inner.propertyPath.Count(c => c == '.') == dots + 1;
                if (isDirectChild)
                {
                    yield return inner;
                }
            }
        }
    }
}
