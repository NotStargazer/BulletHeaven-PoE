using UnityEngine;

namespace Utility
{
    public static class VectorExtensions
    {
        public static Vector2 ToXZ(this Vector3 a)
        {
            return new Vector2(a.x, a.z);
        }
    }
}
