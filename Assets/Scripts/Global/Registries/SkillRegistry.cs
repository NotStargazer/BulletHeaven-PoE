using System;
using Gameplay;
using Gameplay.Skills;
using UI.Localization;

namespace Global.Registries
{
#if UNITY_EDITOR
    public static class SkillsLocalization
    {
        [UnityEditor.MenuItem("Localization/Reload/Skills")]
        private static void RunStatLocalization()
        {
            LocalizationKeyGenerator.OpenEntry("Skills");
            {
                foreach (var fieldInfo in typeof(SkillRegistry).GetFields())
                {
                    LocalizationKeyGenerator.AddKey($"Skills.Name.{fieldInfo.Name}");
                    LocalizationKeyGenerator.AddKey($"Skills.Description.{fieldInfo.Name}");
                }
            }
            LocalizationKeyGenerator.CloseEntry();
        }
    }
#endif
    
    [Serializable]
    public class SkillRegistry : Registry<Skill>
    {
        public RegistryEntry<FireBall> FireBall;
        public RegistryEntry<ChainLightning> ChainLightning;
        public RegistryEntry<Slash> Slash;
    }
}
