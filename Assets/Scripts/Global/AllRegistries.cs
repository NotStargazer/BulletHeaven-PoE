using System;
using Global.Registries;
using Utility;

namespace Global
{
    [Flags]
    public enum SkillTags
    {
        Melee = 1 << 0,
        Attack = 1 << 1,
        Spell = 1 << 2,
        Projectile = 1 << 3,
        Minion = 1 << 4,
    }
    
    public class AllRegistries : SingletonBehaviour<AllRegistries>
    {
        public override void Instantiate()
        {
            Skills.CreateRegistry();
            Collectables.Initialize();
        }
        public SkillRegistry Skills;
        public SupportRegistry Supports;
        public CollectableRegistry Collectables;
    }
}
