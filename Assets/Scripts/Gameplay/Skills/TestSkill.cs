using System;
using Gameplay.Data;
using Gameplay.Projectiles;
using UnityEngine;

namespace Gameplay.Skills
{
    [Serializable]
    public class TestSkill : Skill
    {
        [SerializeField] private SimpleProjectile _projectile;
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private float _projectileRange;
        [SerializeField] private int _projectilePierce;
        [SerializeField] private FlatDamage _baseDamage;
        [SerializeField] private StatType[] _damageScalers;
        private InstancePool<Projectile> _projectilePool;
        
        protected override void Construct()
        {
            _projectilePool = new InstancePool<Projectile>(_projectile, 20, 10, 100);
        }

        protected override void Use(IEntity user, Vector3 aimPosition, Support[] supports)
        {
            var damage = user.StatTracker.CalculateDamage(_baseDamage, this, _damageScalers);
            var penetration = user.StatTracker.GetResistancePenetration(this);
            var projectile = _projectilePool.Rent();
            
            projectile.Fire(user, user.Transform.forward, _projectileSpeed, _projectileRange, _projectilePierce);
            projectile.CollisionEvent += hits =>
            {
                foreach (var hitEntity in hits)
                {
                    hitEntity.HitPool.ApplyDamage(damage, penetration);
                }
            };
        }
    }
}
