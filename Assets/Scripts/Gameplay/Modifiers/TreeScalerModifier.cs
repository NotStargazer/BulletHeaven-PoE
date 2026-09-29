using System;
using Gameplay.Data;
using UnityEngine;

namespace Gameplay.Modifiers
{
    [Serializable]
    public class TreeScalerModifier : TreeModifier
    {
        [SerializeField] private StatType[] _scalers;
        [SerializeField] private StatType _finalType;
        
        public override Stat[] Calculate(IEntity entity, Skill skill = null)
        {
            var scalers = entity.StatTracker.GetStats(skill, _scalers);
            scalers = Stat.Transform(_finalType, scalers);
            scalers = Stat.IsolateFactor(StatFactor.Increased, scalers);
        }
    }
}