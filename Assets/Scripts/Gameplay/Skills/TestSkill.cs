using System;
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
        private InstancePool<Projectile> _projectilePool;
        
        protected override void Construct()
        {
            _projectilePool = new InstancePool<Projectile>(_projectile, 20, 10, 100);
        }

        protected override void Use(Player player, Support[] supports)
        {
            var projectile = _projectilePool.Rent();
            projectile.Fire(player, player.transform.forward, _projectileVelocity, _projectileDuration);
        }
    }
}
