using System;
using Gameplay.Data;
using Gameplay.Modifiers;
using Gameplay.Projectiles;
using UnityEngine;

namespace Gameplay.Skills
{
    [Serializable]
    public class TestSkill : Skill
    {
        [SerializeField] private SimpleProjectile _projectile;
        [SerializeField] private float _projectileVelocity;
        [SerializeField] private float _projectileDuration;
        [SerializeField] private FlatDamage _baseDamage;
        [SerializeField] private StatType[] _damageScalers;
        private InstancePool<Projectile> _projectilePool;
        
        protected override void Construct()
        {
            _projectilePool = new InstancePool<Projectile>(_projectile, 20, 10, 100);
        }

        protected override void Use(Player player, Support[] supports)
        {
            var projectile = _projectilePool.Rent();
            var stats = player.GetStats(this, _damageScalers);
            var damage = new DamageSnapshot(Stat.CalculateStats(_baseDamage, stats));
            projectile.Fire(player, player.transform.forward, _projectileVelocity, _projectileDuration);
            projectile.CollisionEvent.AddListener(hits =>
            {
                foreach (var damagable in hits)
                {
                    damagable.ApplyDamage(damage);
                }
            });
        }
    }
}
