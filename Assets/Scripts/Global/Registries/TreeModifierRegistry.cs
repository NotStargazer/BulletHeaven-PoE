using System;
using Gameplay.Modifiers;
using Utility.Attributes;

namespace Global.Registries
{
    [Serializable]
    public class TreeModifierRegistry : Registry<TreeModifier>
    {
        //Power
        
        //Big
        [RegistryFilter("Big")] public RegistryEntry<TreeModifier> TenseStrings;
        
        //Small
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> ProjectileDamage;
        [RegistryFilter("Small")] public RegistryEntry<TreeModifier> AttackSpeed;

        //Travel
        [RegistryFilter("Travel")] public RegistryEntry<TreeModifier> TravelNode;
    }
}
