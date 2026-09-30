using System;
using Global.Registries;
using Utility;

namespace Global
{
    public class AllRegistries : SingletonBehaviour<AllRegistries>
    {
        public override void Instantiate()
        {
            Skills.CreateRegistry();
            SkillTreeModifiers.CreateRegistry();
            Collectables.Initialize();
        }
        public SkillRegistry Skills;
        public SupportRegistry Supports;
        
        public CollectableRegistry Collectables;

        public SkillTreeModifierRegistry SkillTreeModifiers;
    }
}
