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
                user.StatTracker.GetStats(this, new StatType(BaseType.Skill, SkillTags.Area)));
            
            var projectile = _projectilePool.Rent();
            
            projectile.Fire(user, user.Transform.forward, projStats);
            projectile.CollisionEvent += hits =>
            {
                foreach (var hitEntity in hits)
                {
                    hitEntity.HitPool.ApplyDamage(projDamage, penetration);
                }
            };
            projectile.DestroyEvent += projectileTransform =>
            {
                Explode(projectileTransform, area, areaDamage, penetration);
            };
        }

        private void Explode(Transform transform, float radius, DamageSnapshot damage, PenetrationSnapshot penetration)
        {
            Array.Clear(_colliders, 0, _colliders.Length);
            Physics.OverlapSphereNonAlloc(transform.position, radius, _colliders, (int)DefinedLayers.Targetable);
            //_fireballExplosion.Rent().Play();
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
