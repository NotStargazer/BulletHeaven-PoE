using UI.Components;
using UnityEditor;
using UnityEngine;

namespace Editor.Tests
{
    public static class Tests
    {
        private struct TestHash
        {
            public int Big;
            public int Small;

            public override int GetHashCode()
            {
                return unchecked(Big * 31 + Small);
            }
        }
        
        [MenuItem("Tests/Test Hashcode")]
        public static void TestHashCodes()
        {
            var test1 = new TestHash()
            {
                Big = 3,
                Small = 4
            };
            var test2 = new TestHash()
            {
                Big = 2,
                Small = 3
            };
            Debug.Log($"Test 1: {test1.GetHashCode()}, Test 2: {test2.GetHashCode()}");
        }
    }
}
