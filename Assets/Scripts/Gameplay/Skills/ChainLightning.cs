using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Data;
using Gameplay.Modifiers;
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
        [SerializeField] private float _autoAimRadius;
        [SerializeField] private float _chainRadius;
        [SerializeField] private int _chains;
        [SerializeField] private int _maximumTargets;
        [SerializeField] private float _velocity;
        [SerializeField] private FlatDamage _baseDamage;
        [SerializeField] private StatType[] _damageScalers;
        private InstancePool<VisualEffect<LightningBoltData>> _vfxPool;
        private Collider[] _colliders;
        private List<Target> _targets;
        
        protected override void Construct()
        {
            _vfxPool = new InstancePool<VisualEffect<LightningBoltData>>(_lightningVFX, 10, 5, 30);
            _colliders = new Collider[_maximumTargets];
            _targets = new List<Target>(_maximumTargets);
        }

        protected override void Use(IEntity user, Vector3 aimPosition, Support[] supports)
        {
            var numTargets = Stat.CalculateStats(1,
                user.StatTracker.GetStats(this, new StatType(BaseType.Skill, SkillStat.Targets)));
            var numChains = Stat.CalculateStats(_chains,
                user.StatTracker.GetStats(this, new StatType(BaseType.Skill, SkillStat.Chain)));
            var damage = user.StatTracker.CalculateDamage(_baseDamage, this, _damageScalers);
            var penetration = user.StatTracker.GetResistancePenetration(this);
            
            Array.Clear(_colliders, 0, _colliders.Length);
            Physics.OverlapSphereNonAlloc(aimPosition, _autoAimRadius, _colliders, user.SkillManager.TargetLayers);
            _targets.Clear();
            foreach (var collider in _colliders)
            {
                if (!collider)
                {
                    break;
                }
                
                if (!collider.transform.TryGetComponent(out IEntity e))
                {
                    continue;
                }

                var hitDistance = collider.transform.position.SqrDistanceTo(aimPosition);
                _targets.Add(new Target(e, hitDistance));
            }

            var targets = _targets.OrderBy(x => x).Take(numTargets);
            if (targets.Count() == 0)
            {
                var distanceToTarget = Vector3.Distance(user.SkillManager.SkillSpawnPosition, aimPosition);
                var duration = distanceToTarget / _velocity;
                _vfxPool.Rent().Play(new LightningBoltData
                {
                    Duration = duration,
                    Source = user.SkillManager.SkillSpawnPosition,
                    Target = aimPosition,
                    HasTarget = false,
                });
            }
            
            foreach(var target in targets)
            {
                var targetPosition = target.Entity.transform.position;
                var distanceToTarget = Vector3.Distance(user.SkillManager.SkillSpawnPosition, targetPosition);

                var duration = distanceToTarget / _velocity;
                _vfxPool.Rent().Play(new LightningBoltData
                {
                    Duration = duration,
                    Source = user.SkillManager.SkillSpawnPosition,
                    Target = targetPosition,
                    HasTarget = true,
                });

                var chain = EventManager.CreateChain();
                chain.AddEvent(duration,
                    EventManager.ExecutionOrder.AfterDuration,
                    () => Chain(user, target.Entity, damage, penetration, chain, numChains, new HashSet<IEntity> { target.Entity }));
            }
        }

        private void Chain(IEntity user, IEntity target, DamageSnapshot damage, PenetrationSnapshot penetration,
            EventManager.EventChain chain, int chains, HashSet<IEntity> hitTargets)
        {
            var targetPosition = target.transform.position;
            target.HitPool.ApplyDamage(damage, penetration);

            if (chains == 0)
            {
                chain.Complete();
                return;
            }
            
            Array.Clear(_colliders, 0, _colliders.Length);
            Physics.OverlapSphereNonAlloc(targetPosition, _chainRadius, _colliders, user.SkillManager.TargetLayers);
            var distanceFromNext = Mathf.Infinity;
            IEntity nextTarget = null;
            foreach (var collider in _colliders)
            {
                if (!collider)
                {
                    break;
                }
                var hitDistance = collider.transform.position.SqrDistanceTo(targetPosition);
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
                var distanceToTarget = Vector3.Distance(targetPosition, nextTarget.transform.position);
                var duration = distanceToTarget / _velocity;
                
                _vfxPool.Rent().Play(new LightningBoltData
                {
                    Duration = duration,
                    Source = targetPosition,
                    Target = nextTarget.transform.position,
                    HasTarget = true,
                });
                
                hitTargets.Add(nextTarget);
                chain.AddEvent(duration,
                    EventManager.ExecutionOrder.AfterDuration,
                    () => Chain(user, nextTarget, damage, penetration, chain, chains - 1, hitTargets));
            }
            else
            {
                chain.Complete();
            }
        }
    }
}
