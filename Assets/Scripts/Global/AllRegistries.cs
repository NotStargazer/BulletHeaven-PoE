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
            TreeModifiers.CreateRegistry();
            Clusters.CreateRegistry();
            Collectables.Initialize();
        }
        public SkillRegistry Skills;
        public SupportRegistry Supports;
        
        public CollectableRegistry Collectables;

        public TreeModifierRegistry TreeModifiers;
        public ClusterRegistry Clusters;
    }
}
