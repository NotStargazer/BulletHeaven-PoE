using System;
using System.Collections.Generic;
using Gameplay.Data;
using UnityEngine;
using Utility;

namespace Gameplay
{
    public class Projectile : MonoBehaviour, IPoolInstance<Projectile>
    {
        private uint _id;
        private bool _fired;
        private float _speed;
        private float _range;
        private int _pierce;
        private float _distanceTraveled;
        private Vector3 _direction;
        private LayerMask _targets;
        private HashSet<IEntity> _hits;
        
        public Action<IEntity> CollisionEvent { get; set; }
        public Action<Transform> DestroyEvent { get; set; }
        public Action<Transform> ExpireEvent { get; set; }
        [SerializeField] private ColliderData _collider;
        
        public void Fire(IEntity entity, Vector3 direction, ProjectileSnapshot projectileStats, LayerMask targets, HashSet<IEntity> hits)
        {
            _hits = hits;
            _targets = targets;
            _speed = projectileStats.Speed;
            _direction = direction;
            _range = projectileStats.Range;
            _pierce = projectileStats.Pierce + 1;
            transform.localPosition = entity.SkillManager.SkillSpawnPosition;
            transform.forward = _direction;
            gameObject.SetActive(true);
            _fired = true;
            _distanceTraveled = 0;
        }

        private void Destroy()
        {
            DestroyEvent?.Invoke(transform);
            _fired = false;
            CollisionEvent = null;
            DestroyEvent = null;
            ExpireEvent = null;
            gameObject.SetActive(false);
            Return(this);
        }

        private void Awake()
        {
            _collider.Init();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_fired)
            {
                var timeScaledVelocity = _speed * Time.deltaTime;
                if (_collider.Cast(transform, timeScaledVelocity, _pierce, _targets))
                {
                    var hits = _collider.Hits;
                    foreach (var entity in hits)
                    {
                        if (_hits.Contains(entity))
                        {
                            continue;
                        }
                        _pierce--;
                        CollisionEvent?.Invoke(entity);
                        _hits.Add(entity);

                        if (_pierce > 0)
                        {
                            continue;
                        }
                        Destroy();
                        break;
                    }


                }
                
                var translation = _direction * timeScaledVelocity;
                _distanceTraveled += translation.magnitude;
                if (_distanceTraveled >= _range)
                {
                    Destroy();
                    ExpireEvent?.Invoke(transform);
                    return;
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
