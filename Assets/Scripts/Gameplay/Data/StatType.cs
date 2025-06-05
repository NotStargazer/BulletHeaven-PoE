using System;
using UI.Localization;
using UnityEditor;
using UnityEngine;
using Utility.Attributes;

namespace Gameplay.Data
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class StatLocalization
    {
        static StatLocalization()
        {
            LocalizationKeyGenerator.OpenEntry("Stats");
            {
                foreach (BaseType baseType in Enum.GetValues(typeof(BaseType)))
                {
                    var baseTypeName = baseType.ToString();
                    var field = typeof(BaseType).GetField(baseTypeName);
                    var subType = Attribute.GetCustomAttribute(field, typeof(SubTypeAttribute)) as SubTypeAttribute;
                    var subTypeNames = Enum.GetNames(subType!.SubType);
                    foreach (var subTypeName in subTypeNames)
                    {
                        LocalizationKeyGenerator.AddKey($"{baseTypeName}.Flat.{subTypeName}");
                        LocalizationKeyGenerator.AddKey($"{baseTypeName}.Increased.{subTypeName}");
                        LocalizationKeyGenerator.AddKey($"{baseTypeName}.Multiplier.{subTypeName}");
                        LocalizationKeyGenerator.AddKey($"{baseTypeName}.Fixed.{subTypeName}");
                    }
                }
            }
            LocalizationKeyGenerator.CloseEntry();
        }
    }
#endif

    public enum BaseType
    {
        [SubType(typeof(DamageType))] Damage,
        [SubType(typeof(DamageType))] Resistance,
        [SubType(typeof(ActionType))] Speed,
        [SubType(typeof(ResourceType))] Resource,
        [SubType(typeof(ResourceType))] ResourceRecovery,
        [DisplayNameOverride] [SubType(typeof(DefenceType))] Defence,
    }

    [Flags]
    public enum DamageType
    {
        [DisplayNameOverride] 
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
        [DisplayNameOverride("Action ")] 
        Generic = Attack | Cast | Move,
        Attack = 1 << 0,
        Cast = 1 << 1,
        [DisplayNameOverride("Movement ")] 
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
        [DisplayNameOverride("Defences")]
        Generic = Armor | Evasion | Barrier,
        Armor = 1 << 0,
        Evasion = 1 << 1,
        Barrier = 1 << 2
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

        public string EditorStatName
        {
            get
            {
                var returnString = string.Empty;
                var field = typeof(BaseType).GetFields()[(int)_baseType + 1];
                var baseTypeName = (DisplayNameOverrideAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayNameOverrideAttribute));
                var subType = (SubTypeAttribute)Attribute.GetCustomAttribute(field, typeof(SubTypeAttribute));
                var subField = subType.SubType.GetFields()[_subType + 1];
                var subTypeName = (DisplayNameOverrideAttribute)Attribute.GetCustomAttribute(subField, typeof(DisplayNameOverrideAttribute));

                returnString += subTypeName != null ? subTypeName.DisplayName : $"{subField.Name} ";
                returnString += baseTypeName != null ? baseTypeName.DisplayName : $"{field.Name}";
                return returnString;
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
    }
}
