using System.Collections.Generic;
using Gameplay.Modifiers;

namespace Gameplay.Data
{
    public struct SpeedSnapshot
    {
        public readonly float BaseTimeMod;
        public readonly float SpeedMultiplier;
        
        public SpeedSnapshot(List<Stat> stats)
        {
            BaseTimeMod = Stat.CalculateFlat(stats);
            SpeedMultiplier = Stat.CalculateMultiplier(stats);
        }
    }
}