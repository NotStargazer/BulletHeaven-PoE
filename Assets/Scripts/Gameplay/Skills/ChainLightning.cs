using System;
using System.Collections.Generic;
using Gameplay.Data;
using Gameplay.Projectiles;
using UnityEngine;
using Utility;
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
        
        protected override void Construct()
        {
            _vfxPool = new InstancePool<VisualEffect<LightningBoltData>>(_lightningVFX, 10, 5, 30);
        }

        protected override void Use(IEntity entity, Vector3 aimPosition, Support[] supports)
        {
            var colliders = new Collider[20];
            Physics.OverlapSphereNonAlloc(aimPosition, _autoAimRadius, colliders, (int)DefinedLayers.Targetable);
            var distance = Mathf.Infinity;
            IEntity targetEntity = null;
            foreach (var collider in colliders)
            {
                if (!collider)
                {
                    break;
                }
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
                HasTarget = targetEntity != null,
            });

            if (targetEntity == null)
            {
                return;
            }

            var chain = EventManager.CreateChain();
            chain.AddEvent(0.05f, 
                EventManager.ExecutionOrder.AfterDuration,
                () => Chain(entity, targetEntity, chain, _chains, new HashSet<IEntity>{targetEntity}));
        }

        private void Chain(IEntity source, IEntity target, EventManager.EventChain chain, int chains, HashSet<IEntity> hitTargets)
        {
            var damage = source.StatTracker.CalculateDamage(_baseDamage, this, _damageScalers);
            target.HitPool.ApplyDamage(damage);
            var position = target.Transform.position;

            if (chains == 0)
            {
                chain.Complete();
                return;
            }
            
            var colliders = new Collider[20];
            Physics.OverlapSphereNonAlloc(position, _chainRadius, colliders, (int)DefinedLayers.Targetable);
            var distance = Mathf.Infinity;
            IEntity nextTarget = null;
            foreach (var collider in colliders)
            {
                if (!collider)
                {
                    break;
                }
                var hDistance = Vector3.Distance(collider.transform.position, position);
                if (hDistance < distance)
                {
                    if (collider.transform.TryGetComponent(out IEntity hitEntity))
                    {
                        if (!hitTargets.Contains(hitEntity))
                        {
                            nextTarget = hitEntity;
                            distance = hDistance;
                        }
                    }
                }
            }

            if (nextTarget != null)
            {
                hitTargets.Add(nextTarget);
                chain.AddEvent(0.05f,
                    EventManager.ExecutionOrder.AfterDuration,
                    () =>
                    {
                        _vfxPool.Rent().Play(new LightningBoltData
                        {
                            Source = target.Transform.position,
                            Target = nextTarget.Transform.position,
                            HasTarget = true,
                        });
                        Chain(source, nextTarget, chain, chains - 1, hitTargets);
                    });
            }
            else
            {
                chain.Complete();
            }
        }
    }
}
