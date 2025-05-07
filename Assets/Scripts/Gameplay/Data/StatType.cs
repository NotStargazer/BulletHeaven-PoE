using System;
using UnityEngine;
using Utility.Attributes;

namespace Gameplay.Data
{
    public enum BaseType
    {
        [SubType(typeof(DamageType))] Damage,
        [SubType(typeof(DamageType))] Resistance,
        [SubType(typeof(ResourceType))] Resource
    }
    
    public enum DamageType
    {
        Physical,
        Fire,
        Cold,
        Lightning
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
    }
}
