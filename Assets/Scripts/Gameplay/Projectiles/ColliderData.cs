using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Projectiles
{
    [Serializable]
    public class ColliderData
    {
        public enum ColliderType
        {
            Sphere,
            Capsule,
        }

        [SerializeField] private ColliderType _type;
        [SerializeField] private float _size;
        [SerializeField] private float _length;
        
        public bool Cast(Transform transform, float distance, int maxHits, out List<IEntity> hits)
        {
            RaycastHit[] raycastHits = new RaycastHit[maxHits];
            switch (_type)
            {
                case ColliderType.Sphere:
                    Physics.SphereCastNonAlloc(new Ray(transform.localPosition, transform.forward), _size, raycastHits, distance);
                    break;
                case ColliderType.Capsule:
                    Physics.CapsuleCastNonAlloc(
                        transform.position - transform.forward * _length,
                        transform.position + transform.forward * _length,
                        _size, transform.forward, raycastHits, distance);
                    break;
            }
            
            hits = new List<IEntity>();
            foreach (var hit in raycastHits)
            {
                if (!hit.transform)
                {
                    continue;
                }
                if (hit.transform.TryGetComponent<IEntity>(out var component))
                {
                    hits.Add(component);
                }
            }

            return hits.Count > 0;
        }

        public void DrawGizmo(Transform transform)
        {
            switch (_type)
            {
                case ColliderType.Sphere:
                    break;
                case ColliderType.Capsule:
                    Gizmos.DrawWireSphere(transform.position - transform.forward * _length, _size);
                    Gizmos.DrawWireSphere(transform.position + transform.forward * _length, _size);
                    break;
            }
        }
    }
}
