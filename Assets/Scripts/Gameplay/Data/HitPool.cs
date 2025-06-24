using System;
using Gameplay.Modifiers;
using UnityEngine;
using Utility;

namespace Gameplay.Data
{
    public class HitPool : MonoBehaviour
    {
        [SerializeField] private InterfaceRef<IEntity> _entity;

        private float _health;
        private float _barrier;

        public Action OnKill { get; set; }

        public bool Alive => _health > 0;

        public void ApplyDamage(DamageSnapshot damage)
        {
            _health -= _entity.I.StatTracker.CalculateResistances(damage);
            
            if (_health <= 0)
            {
                OnKill();
            }
        }

        private void FixedUpdate()
        {
            
        }

        private void ApplyRegen()
        {
            var healthRecoveryStats = _entity.I.StatTracker.GetStats(statTypes: new StatType(BaseType.ResourceRecovery, ResourceType.Life));
        }

        public void Restore()
        {
            var healthStats = _entity.I.StatTracker.GetStats(statTypes: new StatType(BaseType.Resource, ResourceType.Life));
            var barrierStats = _entity.I.StatTracker.GetStats(statTypes: new StatType(BaseType.Defence, DefenceType.Barrier));
            _health = Stat.CalculateStats(healthStats);
            _barrier = Stat.CalculateStats(barrierStats);
        }
    }
}
