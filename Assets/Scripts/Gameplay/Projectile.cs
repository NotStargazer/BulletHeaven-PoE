using System;
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
        
        public Action CollisionEvent { private get; set; }
        public Action ExpireEvent { private get; set; }
        [SerializeField] private ColliderData _collider;
        
        public void Fire(Player player, Vector3 direction, float velocity, float duration)
        {
            _velocity = velocity;
            _direction = direction;
            _expireTime = duration + Time.time;
            transform.localPosition = player.SkillSpawnPosition;
            transform.forward = _direction;
            gameObject.SetActive(true);
            _fired = true;
        }

        private void Destroy()
        {
            _fired = false;
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
                if (_collider.Cast(transform, timeScaledVelocity, 1, out var hits))
                {
                    for (var i = 0; i < 1; i++)
                    {
                        hits[i].ApplyDamage();
                    }
                    
                    Destroy();
                    CollisionEvent?.Invoke();
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
