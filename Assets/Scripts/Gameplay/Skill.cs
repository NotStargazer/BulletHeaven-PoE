using System;
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
        
        protected abstract void Use(Player player, Support[] supports);

        public bool TryUse(Player player, Support[] supports)
        {
            if (_skillCooldown.Check(Time.time, 1))
            {
                Use(player, supports);
                return true;
            }

            return false;
        }
        
        public object GetInstance()
        {
            var skill = (Skill)MemberwiseClone();
            skill.Construct();
            return skill;
        }
    }
}
