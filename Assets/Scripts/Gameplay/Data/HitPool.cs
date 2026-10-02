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
        public float LastDamageTime { get; private set; }

        public void ApplyDamage(DamageSnapshot damage, PenetrationSnapshot penetration)
        {
            _health -= _entity.I.StatTracker.CalculateResistances(damage, penetration);
            
            if (_health <= 0)
            {
                OnKill();
            }

            LastDamageTime = Time.time;
        }

        private void FixedUpdate()
        {
            var hitPool = _entity.I.StatTracker.GetHitPoolStats();
            var lifeRecovery = Time.fixedDeltaTime * hitPool.LifeRegen;
            _health = Mathf.Min(_health + lifeRecovery, hitPool.MaxLife);

            if (LastDamageTime + hitPool.BarrierStartTime <= Time.time)
            {
                var barrierRecovery = Time.fixedDeltaTime * hitPool.MaxBarrier * (1 / hitPool.BarrierRecoveryRate);
                _barrier = Mathf.Min(_barrier + barrierRecovery, hitPool.MaxBarrier);
            }
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
