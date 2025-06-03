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

        private void Awake()
        {
            var healthStats = _entity.I.StatTracker.GetStats(statTypes: new StatType(BaseType.Resource, ResourceType.Life));
            var barrierStats = _entity.I.StatTracker.GetStats(statTypes: new StatType(BaseType.Defence, DefenceType.Barrier));
            _health = Stat.CalculateStats(0, healthStats);
            _barrier = Stat.CalculateStats(0, barrierStats);
        }

        public void ApplyDamage(DamageSnapshot damage)
        {
            _entity.I.StatTracker.CalculateResistance(damage);
            
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
    }
}
