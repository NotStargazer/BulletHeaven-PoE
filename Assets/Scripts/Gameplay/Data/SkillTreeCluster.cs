using System;
using Global;
using Global.Registries;
using UnityEngine;
using Utility.Attributes;

namespace Gameplay.Data
{
    [Serializable]
    public class SkillTreeCluster : IRegistrableData
    {
        [RegistryDropdown(typeof(TreeModifierRegistry), "Big")]
        [SerializeField] public string[] _bigSkills;
        [RegistryDropdown(typeof(TreeModifierRegistry), "Small")]
        [SerializeField] public string[] _smallSkills;
        
        public override int GetHashCode()
        {
            return unchecked(_bigSkills.Length * 31 + _smallSkills.Length);
        }

        public object GetInstance()
        {
            return (SkillTreeCluster)MemberwiseClone();
        }
    }
}