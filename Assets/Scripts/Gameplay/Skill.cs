using System;
using Gameplay.Modifiers;
using Global;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public abstract class Skill : IRegistrableData
    {
        [SerializeField] protected SkillTags _tags;
        [SerializeField] protected CooldownTimer _skillCooldown;
        protected abstract void Construct();
        
        protected abstract void Use(IEntity entity, Support[] supports);

        public bool TryUse(IEntity entity, Support[] supports)
        {
            if (_skillCooldown.Check(Time.time, 1))
            {
                Use(entity, supports);
                return true;
            }

            return false;
        }

        public bool CheckTags(SkillTags compare)
        {
            return (_tags & compare) == compare;
        }
        
        public object GetInstance()
        {
            var skill = (Skill)MemberwiseClone();
            skill.Construct();
            return skill;
        }
    }
}
