using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.Data
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

        private List<IEntity> _hits;

        private RaycastHit[] _raycastHits;
        private Collider[] _overlaps;

        public IReadOnlyList<IEntity> Hits => _hits;

        private void ClearAndCheckSize<T>(int size, ref T[] array)
        {
            Array.Clear(array, 0, array.Length);
            if (size > array.Length)
            {
                Array.Resize(ref array, size);
            }
        }
        
        public bool Cast(Transform transform, float distance, int maxHits, int mask = 0)
        {
            _hits.Clear();
            ClearAndCheckSize(maxHits, ref _raycastHits);
            switch (_type)
            {
                case ColliderType.Sphere:
                    Physics.SphereCastNonAlloc(new Ray(transform.localPosition, transform.forward),
                        _size, _raycastHits, distance, mask);
                    break;
                case ColliderType.Capsule:
                    Physics.CapsuleCastNonAlloc(
                        transform.position - transform.forward * _length,
                        transform.position + transform.forward * _length,
                        _size, transform.forward, _raycastHits, distance, mask);
                    break;
            }
            
            foreach (var hit in _raycastHits)
            {
                if (!hit.transform)
                {
                    continue;
                }
                if (hit.transform.TryGetComponent<IEntity>(out var component))
                {
                    _hits.Add(component);
                }
            }

            return _hits.Count > 0;
        }

        public bool Overlap(Transform transform, int maxHits, int mask = 0)
        {
            _hits.Clear();
            ClearAndCheckSize(maxHits, ref _overlaps);
            switch (_type)
            {
                case ColliderType.Sphere:
                    Physics.OverlapSphereNonAlloc(transform.position, _size, _overlaps, mask);
                    break;
                case ColliderType.Capsule:
                    Physics.OverlapCapsuleNonAlloc(
                        transform.position - transform.forward * _length,
                        transform.position + transform.forward * _length, _size, _overlaps, mask);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_type));
            }
            
            foreach (var overlap in _overlaps)
            {
                if (!overlap.transform)
                {
                    continue;
                }
                if (overlap.transform.TryGetComponent<IEntity>(out var component))
                {
                    _hits.Add(component);
                }
            }

            return _hits.Count > 0;
        }

        public void DrawGizmo(Transform transform)
        {
            switch (_type)
            {
                case ColliderType.Sphere:
                    Gizmos.DrawWireSphere(transform.position, _size);
                    break;
                case ColliderType.Capsule:
                    Gizmos.DrawWireSphere(transform.position - transform.forward * _length, _size);
                    Gizmos.DrawWireSphere(transform.position + transform.forward * _length, _size);
                    break;
            }
        }

        public void Init()
        {
            _hits = new List<IEntity>();
            _raycastHits = new RaycastHit[1];
            _overlaps = new Collider[1];
        }
    }
}
