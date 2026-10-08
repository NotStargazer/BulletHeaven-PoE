using UnityEngine;

namespace Utility
{
    public static class VectorExtensions
    {
        public static Vector2 ToXZ(this Vector3 a)
        {
            return new Vector2(a.x, a.z);
        }
        
        public static Vector3 ToXZ3(this Vector3 a)
        {
            return new Vector3(a.x, 0, a.z);
        }

        public static float SqrDistanceTo(this Vector3 a, Vector3 b)
        {
            var num1 = a.x - b.x;
            var num2 = a.y - b.y;
            var num3 = a.z - b.z;
            return (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3);
        }
    }
}
