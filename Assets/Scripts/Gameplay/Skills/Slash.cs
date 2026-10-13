using System;
using Gameplay.Data;
using Gameplay.Modifiers;
using UnityEngine;
using Visuals;
using Visuals.VisualEffects;

namespace Gameplay.Skills
{
    [Serializable]
    public class Slash : Skill
    {
        [SerializeField] private SlashVFX _slashVFX;
        [SerializeField] private float _strikeRadius;
        [SerializeField] private int _maxOverlaps;
        [SerializeField] private FlatDamage _strikeDamage;
        [SerializeField] private StatType[] _damageScalers;
        private InstancePool<VisualEffect<SlashData>> _vfxPool;
        private Collider[] _colliders;
        protected override void Construct()
        {
            _vfxPool = new InstancePool<VisualEffect<SlashData>>(_slashVFX, 1, 2, 20);
            _colliders = new Collider[_maxOverlaps];
        }

        protected override void Use(IEntity user, Vector3 aimPosition, Support[] supports)
        {
            var damage = user.StatTracker.CalculateDamage(_strikeDamage, this, _damageScalers);
            var penetration = user.StatTracker.GetResistancePenetration(this);
            var radius = Stat.CalculateStats(_strikeRadius,
                user.StatTracker.GetStats(this, StatTypes.Skill.Area));
            
            Array.Clear(_colliders, 0, _colliders.Length);
            Physics.OverlapSphereNonAlloc(user.SkillManager.SkillSpawnPosition, radius, _colliders, user.SkillManager.TargetLayers);
            _vfxPool.Rent().Play(new SlashData
            {
                Center = user.SkillManager.SkillSpawnPosition,
                Forward = user.Transform.forward,
                Radius = radius,
            });
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
                
                e.HitPool.ApplyDamage(damage, penetration);
            }
        }
    }
}