using System;
using Gameplay.Data;

namespace Global.Registries
{
    [Serializable]
    public class ClusterRegistry : Registry<SkillTreeCluster>
    {
        public RegistryEntry<SkillTreeCluster> ProjectileDPS;
    }
}