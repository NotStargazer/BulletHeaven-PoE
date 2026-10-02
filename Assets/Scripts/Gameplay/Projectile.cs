using System;
using System.Collections.Generic;
using Gameplay.Projectiles;
using UnityEngine;
using Utility;

namespace Gameplay
{
    public abstract class Projectile : MonoBehaviour, IPoolInstance<Projectile>
    {
        private bool _fired;
        private float _velocity;
        private float _range;
        private int _pierce;
        private Vector3 _direction;
        
        public Action<IEnumerable<IEntity>> CollisionEvent { get; set; }
        public Action DestroyEvent { get; set; }
        public Action ExpireEvent { get; set; }
        [SerializeField] private ColliderData _collider;
        
        public void Fire(IEntity entity, Vector3 direction, float velocity, float range, int pierce)
        {
            _velocity = velocity;
            _direction = direction;
            _range = range;
            _pierce = pierce;
            transform.localPosition = entity.SkillManager.SkillSpawnPosition;
            transform.forward = _direction;
            gameObject.SetActive(true);
            _fired = true;
        }

        private void Destroy()
        {
            DestroyEvent?.Invoke();
            _fired = false;
            CollisionEvent = null;
            DestroyEvent = null;
            ExpireEvent = null;
            gameObject.SetActive(false);
            Return(this);
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_fired)
            {
                if (Time.time >= _range)
                {
                    Destroy();
                    ExpireEvent?.Invoke();
                    return;
                }

                var timeScaledVelocity = _velocity * Time.deltaTime;
                if (_collider.Cast(transform, timeScaledVelocity, _pierce, out var hits, (int)DefinedLayers.Targetable))
                {
                    CollisionEvent?.Invoke(hits);
                    Destroy();
                }
                transform.Translate(_direction * timeScaledVelocity, Space.World);
            }
        }

        public void OnRent()
        {
            
        }

        public Action<Projectile> Return { private get; set; }

        private void OnDrawGizmos()
        {
            _collider.DrawGizmo(transform);
        }
    }
}
