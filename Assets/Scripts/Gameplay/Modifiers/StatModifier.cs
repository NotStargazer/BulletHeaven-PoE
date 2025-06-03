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

        public override string GetEditorDescription()
        {
            var returnString = string.Empty;

            for (var index = 0; index < _stats.Length - 1; index++)
            {
                returnString += _stats[index].GetEditorDescription(_condition != 0 ? " with " + _condition + "s" : string.Empty);
                returnString += "\n";
            }
            returnString += _stats[^1].GetEditorDescription(_condition != 0 ? " with " + _condition + "s" : string.Empty);

            return returnString;
        }
    }
}
