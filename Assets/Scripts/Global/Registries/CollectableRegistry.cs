using System;
using Gameplay;
using Gameplay.Collectables;
using UnityEngine;

namespace Global.Registries
{
    [Serializable]
    public class CollectableRegistry
    {
        private InstancePool<Collectable> _experiencePool;
        
        [SerializeField] private ExperienceCollectable _experience;

        public void Initialize() { }

        public void StartLevel()
        {
            _experiencePool = new InstancePool<Collectable>(_experience, 25, 25, 1000);
        }

        public ExperienceCollectable GetExperience(int experience)
        {
            var ex = (ExperienceCollectable)_experiencePool.Rent();
            ex.Experience = experience;
            return ex;
        }
    }
}
