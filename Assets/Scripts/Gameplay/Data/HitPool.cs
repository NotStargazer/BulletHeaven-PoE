using System;
using UI;
using UnityEngine;
using Utility;

namespace Gameplay.Data
{
    public class HitPool : MonoBehaviour
    {
        [SerializeField] private InterfaceRef<IEntity> _entity;
        
        private float _life;
        private float Life
        {
            get => _life;
            set
            {
                var max = _entity.I.StatTracker.GetHitPoolStats().MaxLife;
                if (max == 0)
                {
                    LifePercentage.Value = 0;
                    _life = 0;
                    return;
                }
                LifePercentage.Value = Mathf.Clamp01(value / max);
                _life = value;
            }
        }
        private float _barrier;
        private float Barrier
        {
            get => _barrier;
            set
            {
                var max = _entity.I.StatTracker.GetHitPoolStats().MaxBarrier;
                if (max <= 0)
                {
                    BarrierPercentage.Value = 0;
                    _barrier = 0;
                    return;
                }
                BarrierPercentage.Value = Mathf.Clamp01(value / max);
                _barrier = value;
            }
        }
        
        public ObservableValue<float> LifePercentage;
        public ObservableValue<float> BarrierPercentage;

        public Action OnKill { get; set; }

        public bool Alive => Life > 0;
        public float LastDamageTime { get; private set; }

        private void Awake()
        {
            LifePercentage = new ObservableValue<float>(1);
            BarrierPercentage = new ObservableValue<float>(1);
        }

        public void ApplyDamage(DamageSnapshot damage, PenetrationSnapshot penetration)
        {
            float damageVal = _entity.I.StatTracker.CalculateResistances(damage, penetration);
            if (damageVal == 0)
            {
                return;
            }

            var b = Barrier;
            b -= damageVal;
            if (b < 0)
            {
                Life += b;
                b = 0;
            }
            Barrier = b;
            
            if (Life <= 0)
            {
                OnKill();
            }

            LastDamageTime = Time.time;
        }

        private void FixedUpdate()
        {
            var hitPool = _entity.I.StatTracker.GetHitPoolStats();
            var lifeRecovery = Time.fixedDeltaTime * hitPool.LifeRegen;
            Life = Mathf.Min(Life + lifeRecovery, hitPool.MaxLife);

            if (LastDamageTime + hitPool.BarrierStartTime <= Time.time)
            {
                var barrierRecovery = Time.fixedDeltaTime * hitPool.MaxBarrier * (1 / hitPool.BarrierRecoveryRate);
                Barrier = Mathf.Min(Barrier + barrierRecovery, hitPool.MaxBarrier);
            }
        }

        public void Restore()
        {
            var hitPool = _entity.I.StatTracker.GetHitPoolStats();
            Life = hitPool.MaxLife;
            Barrier = hitPool.MaxBarrier;
        }
    }
}
