using System;
using Global;
using UnityEngine;

namespace Gameplay.Modifiers
{
    [Serializable]
    public class StatModifier : Modifier
    {
        [SerializeField] private Stat[] _stats;
        [SerializeField] private SkillTags _condition;
        
        public override Stat[] Calculate(Player player, Skill skill)
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
    }
}
