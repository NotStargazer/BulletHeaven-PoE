using System;
using UnityEngine;
using Utility.Attributes;

namespace Gameplay.Data
{
    public enum BaseType
    {
        [SubType(typeof(DamageType))] Damage,
        [SubType(typeof(SkillType))] SkillDamage,
        [SubType(typeof(DamageType))] Resistance,
        [SubType(typeof(ResourceType))] Resource,
        [SubType(typeof(ResourceType))] ResourceRegen,
        [SubType(typeof(DefenceType))] Defence,
    }
    
    public enum DamageType
    {
        Physical,
        Fire,
        Cold,
        Lightning
    }
    
    public enum SkillType
    {
        Melee,
        Projectile,
        Spell,
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
