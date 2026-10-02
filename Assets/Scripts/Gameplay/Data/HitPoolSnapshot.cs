using Gameplay.Modifiers;

namespace Gameplay.Data
{
    public struct HitPoolSnapshot
    {
        public readonly float MaxLife;
        public readonly float MaxBarrier;
        public readonly float LifeRegen;
        public readonly float BarrierStartTime;
        public readonly float BarrierRecoveryRate;
        
        public HitPoolSnapshot(float maxLife, float maxBarrier, float lifeRegen, float barrierStartTime, float barrierRecoveryRate)
        {
            MaxLife = maxLife;
            MaxBarrier = maxBarrier;
            LifeRegen = lifeRegen;
            BarrierStartTime = barrierStartTime;
            BarrierRecoveryRate = barrierRecoveryRate;
        }
    }
}