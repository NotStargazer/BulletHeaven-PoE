using System;
using Gameplay.Data;
using Gameplay.Modifiers;
using UnityEngine;
using Utility;
using Visuals;
using Visuals.VisualEffects;

namespace Gameplay.Skills
{
    [Serializable]
    public class FireBall : Skill
    {
        [SerializeField] private FireExplosionVFX _explosionVFX;
        [SerializeField] private Projectile _projectile;
        [SerializeField] private int _projectileCount;
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private float _projectileRange;
        [SerializeField] private int _projectilePierce;
        [SerializeField] private float _explosionRadius;
        [SerializeField] private float _projectileSpread;
        [SerializeField] private int _maxOverlaps;
        [SerializeField] private FlatDamage _projectileDamage;
        [SerializeField] private FlatDamage _explosionDamage;
        [SerializeField] private StatType[] _damageScalers;
        private InstancePool<Projectile> _projectilePool;
        private InstancePool<VisualEffect<FireExplosionData>> _fireballExplosion;
        private Collider[] _colliders;
        
        protected override void Construct()
        {
            _fireballExplosion = new InstancePool<VisualEffect<FireExplosionData>>(_explosionVFX, 5, 5, 100);
            _projectilePool = new InstancePool<Projectile>(_projectile, 5, 5, 100);
            _colliders = new Collider[_maxOverlaps];
        }

        protected override void Use(IEntity user, Vector3 aimPosition, Support[] supports)
        {
            var projDamage = user.StatTracker.CalculateDamage(_projectileDamage, this, _damageScalers);
            var areaDamage = user.StatTracker.CalculateDamage(_explosionDamage, this, _damageScalers);
            var penetration = user.StatTracker.GetResistancePenetration(this);
            var projStats = user.StatTracker.GetProjectileStats(_projectileCount, _projectileSpeed, _projectileRange, _projectilePierce);
            var area = Stat.CalculateStats(_explosionRadius,
                user.StatTracker.GetStats(this, new StatType(BaseType.Skill, SkillStat.Area)));
            var single = projStats.Count == 1;
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
                projectile.Fire(user, direction, projStats, user.SkillManager.TargetLayers);
                projectile.CollisionEvent += hits =>
                {
                    foreach (var hitEntity in hits)
                    {
                        hitEntity.HitPool.ApplyDamage(projDamage, penetration);
                    }
                };
                projectile.DestroyEvent += projectileTransform =>
                {
                    Explode(user, projectileTransform, area, areaDamage, penetration);
                };
            }
        }

        private void Explode(IEntity user, Transform transform, float radius, DamageSnapshot damage, PenetrationSnapshot penetration)
        {
            Array.Clear(_colliders, 0, _colliders.Length);
            Physics.OverlapSphereNonAlloc(transform.position, radius, _colliders, user.SkillManager.TargetLayers);
            _fireballExplosion.Rent().Play(new FireExplosionData
            {
                Center = transform.position,
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
