using System;
using Gameplay.Data;
using Gameplay.Modifiers;
using Global;
using UI.Localization;
using UnityEngine;
using Utility;
using Utility.Attributes;

namespace Gameplay
{
#if UNITY_EDITOR
    public static class SkillLocalization
    {
        [UnityEditor.MenuItem("Localization/Reload/Skills")]
        private static void RunSkillLocalization()
        {
            LocalizationKeyGenerator.OpenEntry("Conditions");
            {
                foreach (SkillTags baseType in Enum.GetValues(typeof(SkillTags)))
                {
                    foreach (ConditionalType conditionalType in Enum.GetValues(typeof(ConditionalType)))
                    {
                        LocalizationKeyGenerator.AddKey($"Condition.{conditionalType}.{baseType}");
                    }
                }
            }
            LocalizationKeyGenerator.CloseEntry();
        }
    }
#endif
    
    [Flags]
    public enum SkillTags
    {
        Attack = 1 << 0,
        Spell = 1 << 1,
        Melee = 1 << 2,
        Projectile = 1 << 3,
        Area = 1 << 4,
        Minion = 1 << 5,
    }
    
    [Serializable]
    public abstract class Skill : IRegistrableData
    {
        protected struct Target : IComparable<Target>
        {
            public Target(IEntity entity, float distance)
            {
                Entity = entity;
                _distance = distance;
            }
            
            public IEntity Entity;
            private readonly float _distance;
            public int CompareTo(Target other)
            {
                return _distance > other._distance ? 1 : -1;
            }
        }
        
        [SerializeField] protected SkillTags _tags;
        [SerializeField] protected CooldownTimer _skillCooldown;
        
        protected abstract void Construct();
        
        protected abstract void Use(IEntity user, Vector3 aimPosition, Support[] supports);

        public bool TryUse(IEntity user, Vector3 aimPosition, Support[] supports)
        {
            var speed = user.StatTracker.GetSkillSpeed(this, _tags);
            if (_skillCooldown.Check(Time.time, speed.SpeedMultiplier, speed.BaseTimeMod))
            {
                Use(user, aimPosition, supports);
                var repeats = Stat.CalculateStats(0,
                    user.StatTracker.GetStats(this, new StatType(BaseType.Skill, SkillStat.Repeat)));
                if (repeats > 0)
                {
                    var chain = EventManager.CreateChain();
                    var repeatTime = (_skillCooldown.Duration - speed.BaseTimeMod) / speed.SpeedMultiplier * _skillCooldown.RepeatMultiplier;
                    for (var i = 0; i < repeats; i++)
                    {
                        chain.AddEvent(repeatTime, EventManager.ExecutionOrder.AfterDuration, () =>
                        {
                            Use(user, aimPosition, supports);
                        });
                        chain.Complete();
                    }

                }
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
            var skill = MemberwiseClone() as Skill;
            _skillCooldown.Reset(Time.time);
            skill!.Construct();
            return skill;
        }
    }
}
