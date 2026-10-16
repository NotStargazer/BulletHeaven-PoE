using System;
using System.Collections.Generic;
using Gameplay.Data;
using UnityEngine;
using Utility;
using Visuals;
using Visuals.VisualEffects;

namespace Gameplay.Skills
{
    [Serializable]
    public class ArrowVolley : Skill
    {
        [SerializeField] private HitVFX _hitVFX;
        [SerializeField] private Projectile _projectile;
        [SerializeField] private int _projectileCount;
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private float _projectileRange;
        [SerializeField] private int _projectilePierce;
        [SerializeField] private float _projectileSpread;
        [SerializeField] private FlatDamage _projectileDamage;
        [SerializeField] private StatType[] _damageScalers;
        private InstancePool<Projectile> _projectilePool;
        private InstancePool<VisualEffect<HitSparkData>> _vfxPool;
        
        protected override void Construct()
        {
            _vfxPool = new InstancePool<VisualEffect<HitSparkData>>(_hitVFX, _projectileCount, _projectileCount, 100);
            _projectilePool = new InstancePool<Projectile>(_projectile, _projectileCount * 2, _projectileCount * 2, 100);
        }

        protected override void Use(IEntity user, Vector3 aimPosition, Support[] supports)
        {
            var projDamage = user.StatTracker.CalculateDamage(_projectileDamage, this, _damageScalers);
            var penetration = user.StatTracker.GetResistancePenetration(this);
            var projStats = user.StatTracker.GetProjectileStats(_projectileCount, _projectileSpeed, _projectileRange, _projectilePierce);
            var single = projStats.Count == 1;
            var hashSet = new HashSet<IEntity>(projStats.Count * (projStats.Pierce + 1));
            
            for(var i = 0; i < projStats.Count; i++)
            {
                var direction = user.Transform.forward;
                if (!single)
                {
                    var angle = Mathf.InverseLerp(0, projStats.Count - 1, i);
                    var right = user.Transform.right * (_projectileSpread * (projStats.Count - 1));
                    var faceDirection = aimPosition.ToXZ3() - user.Transform.position.ToXZ3();
                    direction = Vector3.Lerp(-right, right, angle) + faceDirection;
                    direction.Normalize();
                }
                var projectile = _projectilePool.Rent();
                projectile.Fire(user, direction, projStats, user.SkillManager.TargetLayers, hashSet);
                projectile.CollisionEvent += hit =>
                {
                    hit.HitPool.ApplyDamage(projDamage, penetration);
                    Collide(user, projectile.transform);
                };
            }
        }

        private void Collide(IEntity user, Transform transform)
        {
            _vfxPool.Rent().Play(new HitSparkData
            {
                Center = transform.position,
            });
        }
    }
}
