using System;
using System.Collections.Generic;
using Gameplay.Projectiles;
using UnityEngine;

namespace Gameplay
{
    public abstract class Projectile : MonoBehaviour, IPoolInstance<Projectile>
    {
        private bool _fired;
        private float _velocity;
        private float _expireTime;
        private Vector3 _direction;
        
        public Action<IEnumerable<IEntity>> CollisionEvent { get; set; }
        public Action DestroyEvent { get; set; }
        public Action ExpireEvent { get; set; }
        [SerializeField] private ColliderData _collider;
        
        public void Fire(IEntity entity, Vector3 direction, float velocity, float duration)
        {
            _velocity = velocity;
            _direction = direction;
            _expireTime = duration + Time.time;
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
                if (Time.time >= _expireTime)
                {
                    Destroy();
                    ExpireEvent?.Invoke();
                    return;
                }

                var timeScaledVelocity = _velocity * Time.deltaTime;
                //TODO overlap filter
                //TODO replace 1s with pierce
                if (_collider.Cast(transform, timeScaledVelocity, 1, out var hits))
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
