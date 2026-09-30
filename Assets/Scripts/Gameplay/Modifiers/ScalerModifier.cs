using System;
using Gameplay.Data;
using UI.Localization;
using UnityEngine;

namespace Gameplay.Modifiers
{
#if UNITY_EDITOR
    using UnityEditor;

    [InitializeOnLoad]
    public static class StatLocalization
    {
        static StatLocalization()
        {
            LocalizationKeyGenerator.OpenEntry("Scaler Modifier");
            {
                foreach (StatFactor factor in Enum.GetValues(typeof(StatFactor)))
                {
                    LocalizationKeyGenerator.AddKey($"ScaleModifier.{factor}");
                }
            }
            LocalizationKeyGenerator.CloseEntry();
        }
    }
#endif
    
    [Serializable]
    public class ScalerModifier : Modifier
    {
        [SerializeField] private StatType[] _scalers;
        [SerializeField] private StatType _finalType;
        [SerializeField] private float _multiplier = 1;
        [SerializeField] private SkillTags _condition;
        [SerializeField] private StatFactor _factor;

        public override bool IsConditional => true;

        public override Stat[] Calculate(IEntity entity, Skill skill = null)
        {
            if (skill != null && _condition != 0)
            {
                if (skill.CheckTags(_condition))
                {
                    return GetScalers(entity, skill);
                }
            }
            else if (_condition == 0)
            {
                return GetScalers(entity, skill);
            }
            
            return null;
        }

        private Stat[] GetScalers(IEntity entity, Skill skill)
        {
            var scalers = entity.StatTracker.GetStats(skill, _scalers);
            scalers = Stat.Transform(_finalType, scalers);
            scalers = Stat.IsolateFactor(StatFactor.Increased, scalers);
            return Stat.Scale(_multiplier, StatFactor.Increased, scalers);
        }

        public override string GetDescription()
        {
            var hasMultiplier = !Mathf.Approximately(_multiplier, 1);
            var hasCondition = _condition > 0;
            var outDescription = new string[_scalers.Length];
            var modKey = $"ScaleModifier.{_factor}";
            var statTypeKey = $"{_finalType.BaseName}.{_finalType.SubName}";
#if UNITY_EDITOR
            var statTypeLoc = LocalizationManager.GetEditorLocalization("Stat Types", statTypeKey);
#else
            var statTypeLoc = Localization.GetLocalization(statTypeKey);
#endif
            for (var i = 0; i < _scalers.Length; i++)
            {
                var statType = _scalers[i];
                var statScalerKey = $"{statType.BaseName}.{statType.SubName}";
#if UNITY_EDITOR
                var statScalerLoc = LocalizationManager.GetEditorLocalization("Stat Types", statScalerKey);
                var modLoc = LocalizationManager.GetEditorLocalization("Scaler Modifier", modKey);
#else
                var statScalerLoc = Localization.GetLocalization(statScalerKey);
                var modLoc = Localization.GetLocalization(modKey);
#endif
                modLoc = modLoc.ReplaceVariable("Scaler", statScalerLoc.Build())
                    .ReplaceVariable("StatType", statTypeLoc.Build())
                    .SetConditional("HasEffect", hasMultiplier)
                    .SetConditional("Conditional", hasCondition);
                if (hasMultiplier)
                {
                    modLoc = modLoc.ReplaceVariable("Multiplier", _multiplier * 100);
                }
                if (hasCondition)
                {
                    modLoc = modLoc.ReplaceVariable("Condition",
                        LocalizationManager.GetCondition(_condition, _finalType.GetConditionalKey(_factor)));
                }
                outDescription[i] = modLoc.Build();
            }

            return string.Concat(outDescription);
        }
    }
}