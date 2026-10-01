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

        //Travel
        [RegistryFilter("Root")] public RegistryEntry<StatModifier> DamageNode;
        [RegistryFilter("Root")] public RegistryEntry<StatModifier> LifeNode;
        [RegistryFilter("Root")] public RegistryEntry<StatModifier> SkillSpeedNode;
    }
}
