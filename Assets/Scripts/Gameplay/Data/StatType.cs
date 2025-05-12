using System;
using UnityEngine;
using Utility.Attributes;

namespace Gameplay.Data
{
    public enum BaseType
    {
        [SubType(typeof(DamageType))] Damage,
        [SubType(typeof(DamageType))] Resistance,
        [SubType(typeof(ActionType))] Speed,
        [SubType(typeof(ResourceType))] Resource,
        [SubType(typeof(ResourceType))] ResourceRecovery,
        [SubType(typeof(DefenceType))] Defence,
    }
    
    public enum DamageType
    {
        Generic,
        Physical,
        Fire,
        Cold,
        Lightning,
        Elemental,
    }

    public enum ActionType
    {
        Generic,
        Attack,
        Cast,
        Move,
    }
    
    public enum ResourceType
    {
        Health,
        Mana,
        Stamina,
        Ammo
    }
    
    public enum DefenceType
    {
        Generic,
        Armor,
        Evasion,
        Barrier
    }
    
    [Serializable]
    public record StatType
    {
        [SerializeField] protected BaseType _baseType;
        [SerializeField] protected int _subType;

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
