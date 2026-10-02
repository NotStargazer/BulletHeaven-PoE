using System;
using Gameplay.Modifiers;
using UI.Localization;
using UnityEngine;
using Utility.Attributes;

namespace Gameplay.Data
{
#if UNITY_EDITOR
    public static class StatLocalization
    {
        [UnityEditor.MenuItem("Localization/Reload/Stats")]
        private static void RunStatLocalization()
        {
            LocalizationKeyGenerator.OpenEntry("Stat Types");
            {
                foreach (BaseType baseType in Enum.GetValues(typeof(BaseType)))
                {
                    var baseTypeName = baseType.ToString();
                    var field = typeof(BaseType).GetField(baseTypeName);
                    if (Attribute.GetCustomAttribute(field, typeof(SubTypeAttribute)) 
                        is not SubTypeAttribute subType)
                    {
                        Debug.LogWarning(baseType + " has no SubType Attribute.");
                        continue;
                    }
                    var subTypeNames = Enum.GetNames(subType.SubType);
                    foreach (var subTypeName in subTypeNames)
                    {
                        LocalizationKeyGenerator.AddKey($"{baseTypeName}.{subTypeName}");
                    }
                }
            }
            LocalizationKeyGenerator.CloseEntry();
            
            LocalizationKeyGenerator.OpenEntry("Stat Modifiers");
            {
                foreach (StatFactor factor in Enum.GetValues(typeof(StatFactor)))
                {
                    LocalizationKeyGenerator.AddKey($"Generic.{factor}");
                }
            }
            LocalizationKeyGenerator.CloseEntry();
            
            
            LocalizationKeyGenerator.OpenEntry("Stat Modifier Overrides");
            {
                foreach (BaseType baseType in Enum.GetValues(typeof(BaseType)))
                {
                    var baseTypeName = baseType.ToString();
                    var field = typeof(BaseType).GetField(baseTypeName);
                    if (Attribute.GetCustomAttribute(field, typeof(SubTypeAttribute)) 
                        is not SubTypeAttribute subType)
                    {
                        Debug.LogWarning(baseType + " has no SubType Attribute.");
                        continue;
                    }
                    var subTypeNames = Enum.GetNames(subType.SubType);
                    foreach (var subTypeName in subTypeNames)
                    {
                        foreach (StatFactor factor in Enum.GetValues(typeof(StatFactor)))
                        {
                            LocalizationKeyGenerator.AddKey($"{baseTypeName}.{factor}.{subTypeName}");
                        }
                    }
                }
            }
            LocalizationKeyGenerator.CloseEntry();
        }
    }
#endif

    public enum BaseType
    {
        [SubType(typeof(DamageType), flatStat: ConditionalType.Direct)] Damage,
        [SubType(typeof(DamageType))] Resistance,
        [SubType(typeof(ActionType))] Speed,
        [SubType(typeof(ResourceType))] Resource,
        [SubType(typeof(ResourceType))] ResourceRecovery,
        [SubType(typeof(DefenceType))] Defence,
        [SubType(typeof(SkillStat))] Skill,
        [SubType(typeof(ProjectileStat), flatStat: ConditionalType.Flat)] Projectile,
        [SubType(typeof(BarrierStat))] Barrier,
        [SubType(typeof(DamageType))] Penetration,
    }

    [Flags]
    public enum DamageType
    {
        Generic = Physical | Fire | Cold | Lightning,
        Elemental = Fire | Cold | Lightning,
        Physical = 1 << 0,
        Fire = 1 << 1,
        Cold = 1 << 2,
        Lightning = 1 << 3,
    }

    [Flags]
    public enum ActionType
    {
        Generic = Attack | Cast | Move,
        Skill = Attack | Cast,
        Attack = 1 << 0,
        Cast = 1 << 1,
        Move = 1 << 2,
    }

    [Flags]
    public enum ResourceType
    {
        Life = 1 << 0,
        Mana = 1 << 1,
        Stamina = 1 << 2,
        Ammo = 1 << 3,
    }

    [Flags]
    public enum DefenceType
    {
        Generic = Armor | Evasion | Barrier,
        Armor = 1 << 0,
        Evasion = 1 << 1,
        Barrier = 1 << 2,
    }

    [Flags]
    public enum SkillStat
    {
        Repeat = 1 << 0,
        Targets = 1 << 1,
        Chain = 1 << 2,
        Area = 1 << 3,
    }
    
    [Flags]
    public enum ProjectileStat
    {
        Count = 1 << 0,
        Speed = 1 << 1,
        Range = 1 << 2,
        Pierce = 1 << 3,
    }
    
    [Flags]
    public enum BarrierStat
    {
        StartTime = 1 << 0,
        RegenRate = 1 << 1,
    }

    [Serializable]
    public record StatType
    {
        public StatType(BaseType baseType, Enum subType)
        {
            _baseType = baseType;
            _subType = Convert.ToInt32(subType);
        }
        
        [SerializeField] protected BaseType _baseType;
        [SerializeField] protected int _subType;

        public string BaseName => typeof(BaseType).GetFields()[(int)_baseType + 1].Name;
        public string SubName => Enum.GetName(SubTypeAttribute.SubType, _subType);
        public SubTypeAttribute SubTypeAttribute
        {
            get
            {
                var field = typeof(BaseType).GetFields()[(int)_baseType + 1];
                var subType = (SubTypeAttribute)Attribute.GetCustomAttribute(field, typeof(SubTypeAttribute));
                return subType;
            }
        }


        public virtual bool Equals(StatType other)
        {
            return other._baseType == _baseType && (other._subType & _subType) != 0;
        }

        public override int GetHashCode()
        {
            return unchecked((int)_baseType * 31 + _subType);
        }

        public string GetConditionalKey(StatFactor factor)
        {
            return SubTypeAttribute.GetConditionalKey(factor);
        }
        
        public ConditionalType GetConditional(StatFactor factor)
        {
            return SubTypeAttribute.GetConditional(factor);
        }

        public void Split(Action<StatType> splitFunction)
        {
            var num = _subType;
            for (var i = 0; num != 0; i++)
            {
                if ((num & 1) == 1)
                {
                    splitFunction(
                        new StatType(_baseType, (Enum)Enum.GetValues(SubTypeAttribute.SubType).GetValue(i)));
                }
                num >>= 1;
            }
        }
    }
}
