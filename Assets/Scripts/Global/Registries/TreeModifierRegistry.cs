using System;
using Gameplay.Modifiers;

namespace Global.Registries
{
    [Serializable]
    public class TreeModifierRegistry : Registry<TreeModifier>
    {
        //Power
        
        //Big
        public RegistryEntry<TreeModifier> TenseStrings;
        
        //Small
        public RegistryEntry<TreeModifier> ProjectileDamage;
        public RegistryEntry<TreeModifier> AttackSpeed;

        //Travel
        public RegistryEntry<TreeModifier> TravelNode;
    }
}
