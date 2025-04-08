using System;
using UnityEngine;

namespace Gameplay.Data
{
    public class ExperienceTracker
    {
        private int _level;
        private int _experience;
        private int _experienceRequirement;
        private int _previousExperienceRequirement;
        private readonly int _baseRequirement;
        private readonly float _growthFactor;

        private readonly Action<int, float> _onGainExperience;
        private readonly Action<int> _onLevelUp;
        
        public ExperienceTracker(int baseExperienceRequirement, float growthFactor, Action<int, float> onGainExperience, Action<int> onLevelUp)
        {
            _baseRequirement = baseExperienceRequirement;
            _experienceRequirement = baseExperienceRequirement;
            _previousExperienceRequirement = 0;
            _growthFactor = growthFactor;
            _onGainExperience = onGainExperience;
            _onLevelUp = onLevelUp;
        }

        public void AddExperience(int experience)
        {
            _experience += experience;
            if (_experience >= _experienceRequirement)
            {
                _level++;
                _previousExperienceRequirement = _experienceRequirement;
                _experienceRequirement += Mathf.CeilToInt(_baseRequirement * Mathf.Pow(_level, _growthFactor));
                _onLevelUp?.Invoke(_level);
            }
            
            _onGainExperience?.Invoke(_experience, Mathf.InverseLerp(_previousExperienceRequirement, _experienceRequirement, _experience));
        }
    }
}
