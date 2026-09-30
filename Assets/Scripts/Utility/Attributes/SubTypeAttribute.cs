using System;
using Gameplay.Modifiers;

namespace Utility.Attributes
{
    public enum ConditionalType
    {
        Direct,
        With,
        While,
        Flat,
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class SubTypeAttribute : Attribute
    {
        private readonly ConditionalType _fixedStat;
        private readonly ConditionalType _flatStat;
        private readonly ConditionalType _increasedStat;
        private readonly ConditionalType _multiplierStat;
        public Type SubType;

        public SubTypeAttribute(Type subType, 
            ConditionalType fixedStat = ConditionalType.With,
            ConditionalType flatStat = ConditionalType.With,
            ConditionalType increasedStat = ConditionalType.With,
            ConditionalType multiplierStat = ConditionalType.With)
        {
            _flatStat = flatStat;
            _increasedStat = increasedStat;
            _multiplierStat = multiplierStat;
            _fixedStat = fixedStat;
            SubType = subType;
        }

        public string GetConditionalKey(StatFactor statFactor)
        {
            return statFactor switch
            {
                StatFactor.Fixed => _fixedStat.ToString(),
                StatFactor.Flat => _flatStat.ToString(),
                StatFactor.Increased => _increasedStat.ToString(),
                StatFactor.Multiplier => _multiplierStat.ToString(),
                _ => "With"
            };
        }
        public ConditionalType GetConditional(StatFactor statFactor)
        {
            return statFactor switch
            {
                StatFactor.Fixed => _fixedStat,
                StatFactor.Flat => _flatStat,
                StatFactor.Increased => _increasedStat,
                StatFactor.Multiplier => _multiplierStat,
                _ => ConditionalType.With
            };
        }
    }
}
