using System;
using Gameplay.Data;
using Global;
using UI.Localization;
using UnityEngine;

namespace Gameplay
{
#if UNITY_EDITOR
    using UnityEditor;
    [InitializeOnLoad]
    static class SkillLocalization
    {
        static SkillLocalization()
        {
            LocalizationKeyGenerator.OpenEntry("Conditions");
            {
                foreach (BaseType baseType in Enum.GetValues(typeof(BaseType)))
                {
                    LocalizationKeyGenerator.AddKey($"Condition.{baseType}");
                }
            }
            LocalizationKeyGenerator.CloseEntry();
        }
    }
#endif
    
    [Flags]
    public enum SkillTags
    {
        Melee = 1 << 0,
        Attack = 1 << 1,
        Spell = 1 << 2,
        Projectile = 1 << 3,
        Minion = 1 << 4,
    }
    
    [Serializable]
    public abstract class Skill : IRegistrableData
    {
        [SerializeField] protected SkillTags _tags;
        [SerializeField] protected CooldownTimer _skillCooldown;
        protected abstract void Construct();
        
        protected abstract void Use(IEntity user, Vector3 aimPosition, Support[] supports);

        public bool TryUse(IEntity entity, Vector3 aimPosition, Support[] supports)
        {
            if (_skillCooldown.Check(Time.time, 1))
            {
                Use(entity, aimPosition, supports);
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
