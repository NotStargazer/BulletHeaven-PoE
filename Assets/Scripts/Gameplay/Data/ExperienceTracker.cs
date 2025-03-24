using System;
using UnityEngine;

namespace Gameplay.Data
{
    public class ExperienceTracker
    {
        private int _level;
        private int _experience;
        private int _experienceRequirement;
        private readonly int _baseRequirement;
        private readonly float _growthFactor;

        private readonly Action<int> _onLevelUp;
        
        public ExperienceTracker(int baseExperienceRequirement, float growthFactor, Action<int> onLevelUp)
        {
            _baseRequirement = baseExperienceRequirement;
            _experienceRequirement = baseExperienceRequirement;
            _growthFactor = growthFactor;
            _onLevelUp = onLevelUp;
        }

        public void AddExperience(int experience)
        {
            _experience += experience;
            if (_experience >= _experienceRequirement)
            {
                _level++;
                _experienceRequirement += Mathf.CeilToInt(_baseRequirement * Mathf.Pow(_level, _growthFactor));
                _onLevelUp?.Invoke(_level);
            }
        }
    }
}
