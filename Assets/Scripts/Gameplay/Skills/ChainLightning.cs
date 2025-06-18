using System;
using Gameplay.Data;
using Gameplay.Projectiles;
using UnityEngine;
using Visuals;
using Visuals.VisualEffects;

namespace Gameplay.Skills
{
    [Serializable]
    public class ChainLightning : Skill
    {
        [SerializeField] private LightningBoltVFX _lightningVFX;
        [SerializeField] private ColliderData _collider;
        [SerializeField] private float _autoAimRadius;
        [SerializeField] private float _chainRadius;
        [SerializeField] private int _chains;
        [SerializeField] private FlatDamage _baseDamage;
        [SerializeField] private StatType[] _damageScalers;
        private InstancePool<VisualEffect<LightningBoltData>> _vfxPool;
        private Collider[] _colliders;
        
        protected override void Construct()
        {
            _colliders = new Collider[20];
            _vfxPool = new InstancePool<VisualEffect<LightningBoltData>>(_lightningVFX, 10, 5, 30);
        }

        protected override void Use(IEntity entity, Vector3 aimPosition, Support[] supports)
        {
            Physics.OverlapSphereNonAlloc(aimPosition, _autoAimRadius, _colliders);

            var distance = Mathf.Infinity;
            IEntity targetEntity = null;
            foreach (var collider in _colliders)
            {
                var hDistance = Vector3.Distance(collider.transform.position, aimPosition);
                if (hDistance < distance)
                {
                    collider.transform.TryGetComponent(out targetEntity);
                    distance = hDistance;
                }
            }

            var targetPosition = targetEntity == null ? aimPosition : targetEntity.Transform.position;
            
            _vfxPool.Rent().Play(new LightningBoltData
            {
                Source = entity.SkillManager.SkillSpawnPosition,
                Target = targetPosition,
            });
            
            
        }

        private void Chain(IEntity entity, Vector3 source, int chains, IEntity[] hitTargets)
        {
            
            Physics.OverlapSphereNonAlloc(source, _chainRadius, _colliders);

            var distance = Mathf.Infinity;
            IEntity targetEntity = null;
            foreach (var collider in _colliders)
            {
                var hDistance = Vector3.Distance(collider.transform.position, source);
                if (hDistance < distance)
                {
                    if (collider.transform.TryGetComponent(out IEntity hitEntity))
                    {
                        foreach (var hitTarget in hitTargets)
                        {
                            if (hitTarget == hitEntity)
                            {
                                
                            }
                        }
                    }
                    
                    distance = hDistance;
                }
            }
        }
    }
}
