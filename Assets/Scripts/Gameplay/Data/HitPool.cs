using System;
using Gameplay.Modifiers;
using UnityEngine;
using Utility;

namespace Gameplay.Data
{
    public class HitPool : MonoBehaviour
    {
        [SerializeField] private InterfaceRef<IEntity> _entity;

        private float _life;
        private float _barrier;

        public Action OnKill { get; set; }

        public bool Alive => _life > 0;
        public float LastDamageTime { get; private set; }

        public void ApplyDamage(DamageSnapshot damage, PenetrationSnapshot penetration)
        {
            _life -= _entity.I.StatTracker.CalculateResistances(damage, penetration);
            
            if (_life <= 0)
            {
                OnKill();
            }

            LastDamageTime = Time.time;
        }

        private void FixedUpdate()
        {
            var hitPool = _entity.I.StatTracker.GetHitPoolStats();
            var lifeRecovery = Time.fixedDeltaTime * hitPool.LifeRegen;
            _life = Mathf.Min(_life + lifeRecovery, hitPool.MaxLife);

            if (LastDamageTime + hitPool.BarrierStartTime <= Time.time)
            {
                var barrierRecovery = Time.fixedDeltaTime * hitPool.MaxBarrier * (1 / hitPool.BarrierRecoveryRate);
                _barrier = Mathf.Min(_barrier + barrierRecovery, hitPool.MaxBarrier);
            }
        }

        public void Restore()
        {
            var hitPool = _entity.I.StatTracker.GetHitPoolStats();
            _life = hitPool.MaxLife;
            _barrier = hitPool.MaxBarrier;
        }
    }
}
