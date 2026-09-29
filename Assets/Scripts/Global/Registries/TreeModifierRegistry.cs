using System;
using Gameplay.Modifiers;
using Utility.Attributes;

namespace Global.Registries
{
    [Serializable]
    public class TreeModifierRegistry : Registry<TreeModifier>
    {
        //Dexterity Nodes
        [RegistryFilter("Big")] public RegistryEntry<TreeModifier> BigAdditionalProjectiles;
        [RegistryFilter("Big")] public RegistryEntry<TreeScalerModifier> BigProjectileSpeedDamageScale;
        [RegistryFilter("Big")] public RegistryEntry<TreeScalerModifier> BigMovementSpeedEvasionScale;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> ProjectileDamage;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> ProjectileSpeed;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> ProjectileRange;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> AttackSpeed;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> Evasion;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> MovementSpeed;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> ProjectileDamageAndAttackSpeed;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> ProjectileSpeedAndRange;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> EvasionAndMovementSpeed;

        //Travel
        [RegistryFilter("Travel")] public RegistryEntry<TreeModifier> DamageNode;
    }
}
