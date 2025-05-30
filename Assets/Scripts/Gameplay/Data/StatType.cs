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

    public enum DamageType
    {
        [DisplayNameOverride] Generic,
        Physical,
        Fire,
        Cold,
        Lightning,
        Elemental,
    }

    public enum ActionType
    {
        [DisplayNameOverride("Action ")] Generic,
        Attack,
        Cast,
        [DisplayNameOverride("Movement ")] Move,
    }

    public enum ResourceType
    {
        Life,
        Mana,
        Stamina,
        Ammo
    }

    public enum DefenceType
    {
        [DisplayNameOverride("Defences")] Generic,
        Armor,
        Evasion,
        Barrier
    }

    [Serializable]
    public record StatType
    {
        public StatType(BaseType baseType, Enum subType)
        {
            _baseType = baseType;
            _subType = Convert.ToInt32(subType);
        }
        
        public static StatType[] MultiType<T>(BaseType baseType, params T[] subTypes) where T: Enum
        {
            var types = new StatType[subTypes.Length];
            for (var i = 0; i < subTypes.Length; i++)
            {
                types[i] = new StatType(baseType, subTypes[i]);
            }
            return types;
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
            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return unchecked((int)_baseType * 31 + _subType);
        }
    }
}
