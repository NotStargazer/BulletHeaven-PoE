using Gameplay.Modifiers;

namespace Gameplay.Data
{
    public struct SpeedSnapshot
    {
        public float BaseTimeMod;
        public float SpeedMultiplier;
        
        public SpeedSnapshot(Stat[] stats)
        {
            BaseTimeMod = Stat.CalculateFlat(stats);
            SpeedMultiplier = Stat.CalculateMultiplier(stats);
        }
    }
}