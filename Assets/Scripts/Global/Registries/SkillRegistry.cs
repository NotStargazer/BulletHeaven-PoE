using System;
using Gameplay;
using Gameplay.Skills;

namespace Global.Registries
{
    [Serializable]
    public class SkillRegistry : Registry<Skill>
    {
        public RegistryEntry<FireBall> TestSkill;
        public RegistryEntry<ChainLightning> ChainLightning;
    }
}
