using System;
using Gameplay.Modifiers;
using Utility.Attributes;

namespace Global.Registries
{
    [Serializable]
    public class SkillTreeModifierRegistry : Registry<Modifier>
    {
        //Dexterity Nodes
        [RegistryFilter("Big")] public RegistryEntry<StatModifier> BigAdditionalProjectiles;
        [RegistryFilter("Big")] public RegistryEntry<ScalerModifier> BigProjectileSpeedDamageScale;
        [RegistryFilter("Big")] public RegistryEntry<StatModifier> BigMovementSpeedAndEvasion;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> ProjectileDamage;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> ProjectileSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> ProjectileRange;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> AttackSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> Evasion;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> MovementSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> ProjectileDamageAndAttackSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> ProjectileSpeedAndRange;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> EvasionAndMovementSpeed;
        
        //Strength Nodes
        [RegistryFilter("Big")] public RegistryEntry<StatModifier> BigAdditionalTargets;
        [RegistryFilter("Big")] public RegistryEntry<StatModifier> BigLifeLifeRegenAndArmor;
        [RegistryFilter("Big")] public RegistryEntry<StatModifier> BigAreaOfEffectAndDamage;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> MeleeDamage;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> MeleeAttackSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> Life;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> LifeRegenAndArmor;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> AreaOfEffect;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> AreaDamage;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> MeleeDamageAndAttackSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> LifeAndArmor;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> AreaOfEffectAndDamage;
        
        //Intelligence Nodes
        [RegistryFilter("Big")] public RegistryEntry<StatModifier> BigAdditionalChainsAndElementalDamage;
        [RegistryFilter("Big")] public RegistryEntry<StatModifier> BigSpellAdditionalRepeatsAndCastSpeed;
        [RegistryFilter("Big")] public RegistryEntry<StatModifier> BigBarrierAndElementalResist;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> ElementalDamage;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> ElementalPenetration;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> SpellDamage;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> CastSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> Barrier;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> BarrierRegenAndSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> ElementalDamageAndElementalPenetration;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> SpellDamageAndCastSpeed;
        [RegistryFilter("Small")] public RegistryEntry<StatModifier> BarrierBarrierRegenAndBarrierSpeed;
        
        //Travel
        [RegistryFilter("Root")] public RegistryEntry<StatModifier> DamageNode;
        [RegistryFilter("Root")] public RegistryEntry<StatModifier> LifeNode;
        [RegistryFilter("Root")] public RegistryEntry<StatModifier> SkillSpeedNode;
    }
}
