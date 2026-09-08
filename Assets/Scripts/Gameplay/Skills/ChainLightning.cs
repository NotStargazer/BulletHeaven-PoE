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
        [SerializeField] private int _maximumTargets;
        [SerializeField] private float _velocity;
        [SerializeField] private FlatDamage _baseDamage;
        [SerializeField] private StatType[] _damageScalers;
        private InstancePool<VisualEffect<LightningBoltData>> _vfxPool;
        
        protected override void Construct()
        {
            _vfxPool = new InstancePool<VisualEffect<LightningBoltData>>(_lightningVFX, 10, 5, 30);
        }

        protected override void Use(IEntity user, Vector3 aimPosition, Support[] supports)
        {
            var colliders = new Collider[_maximumTargets];
            Physics.OverlapSphereNonAlloc(aimPosition, _autoAimRadius, colliders, (int)DefinedLayers.Targetable);
            var distanceFromCursor = Mathf.Infinity;
            IEntity targetEntity = null;
            foreach (var collider in colliders)
            {
                if (!collider)
                {
                    break;
                }
                var hitDistance = Vector3.Distance(collider.transform.position, aimPosition);
                if (hitDistance < distanceFromCursor)
                {
                    collider.transform.TryGetComponent(out targetEntity);
                    distanceFromCursor = hitDistance;
                }
            }

            var targetPosition = targetEntity == null ? aimPosition : targetEntity.Transform.position;
            var distanceToTarget = Vector3.Distance(user.SkillManager.SkillSpawnPosition, targetPosition);
            
            var duration = distanceToTarget / _velocity;
            _vfxPool.Rent().Play(new LightningBoltData
            {
                Duration = duration,
                Source = user.SkillManager.SkillSpawnPosition,
                Target = targetPosition,
                HasTarget = targetEntity != null,
            });
            
            if (targetEntity == null)
            {
                return;
            }

            var chain = EventManager.CreateChain();
            chain.AddEvent(duration,
                EventManager.ExecutionOrder.AfterDuration,
                () => Chain(user, targetEntity, chain, _chains, new HashSet<IEntity>{targetEntity}));
        }

        private void Chain(IEntity user, IEntity target, EventManager.EventChain chain, int chains, HashSet<IEntity> hitTargets)
        {
            var damage = user.StatTracker.CalculateDamage(_baseDamage, this, _damageScalers);
            var targetPosition = target.Transform.position;
            target.HitPool.ApplyDamage(damage);

            if (chains == 0)
            {
                chain.Complete();
                return;
            }
            
            var colliders = new Collider[_maximumTargets];
            Physics.OverlapSphereNonAlloc(targetPosition, _chainRadius, colliders, (int)DefinedLayers.Targetable);
            var distanceFromNext = Mathf.Infinity;
            IEntity nextTarget = null;
            foreach (var collider in colliders)
            {
                if (!collider)
                {
                    break;
                }
                var hitDistance = Vector3.Distance(collider.transform.position, targetPosition);
                if (hitDistance < distanceFromNext)
                {
                    if (collider.transform.TryGetComponent(out IEntity hitEntity))
                    {
                        if (!hitTargets.Contains(hitEntity))
                        {
                            nextTarget = hitEntity;
                            distanceFromNext = hitDistance;
                        }
                    }
                }
            }

            if (nextTarget != null)
            {
                var distanceToTarget = Vector3.Distance(targetPosition, nextTarget.Transform.position);
                var duration = distanceToTarget / _velocity;
                
                _vfxPool.Rent().Play(new LightningBoltData
                {
                    Duration = duration,
                    Source = targetPosition,
                    Target = nextTarget.Transform.position,
                    HasTarget = true,
                });
                
                hitTargets.Add(nextTarget);
                chain.AddEvent(duration,
                    EventManager.ExecutionOrder.AfterDuration,
                    () => Chain(user, nextTarget, chain, chains - 1, hitTargets));
            }
            else
            {
                chain.Complete();
            }
        }
    }
}
