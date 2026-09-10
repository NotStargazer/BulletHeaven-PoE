using System;
using UI.Localization;
using UnityEngine;

namespace Gameplay.Modifiers
{
    [Serializable]
    public class StatModifier : Modifier
    {
        [SerializeField] private Stat[] _stats;
        [SerializeField] private SkillTags _condition;

        public override bool IsConditional => _condition != 0;

        public override Stat[] Calculate(IEntity entity, Skill skill = null)
        {
            if (skill != null && _condition != 0)
            {
                if (skill.CheckTags(_condition))
                {
                    return _stats;
                }
            }
            else if (_condition == 0)
            {
                return _stats;
            }

            return null;
        }

        public override string GetDescription()
        {
            var loc = new string[_stats.Length];
            for (var i = 0; i < loc.Length; i++)
            {
                loc[i] = _stats[i].GetDescription(LocalizationManager.GetCondition(_condition));

                if (i != loc.Length - 1)
                {
                    loc[i] += "\n";
                }
            }

            return string.Concat(loc);
        }
    }
}
