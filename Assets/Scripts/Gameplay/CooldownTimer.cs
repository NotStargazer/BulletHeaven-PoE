using System;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public struct CooldownTimer
    {
        [SerializeField] private float _baseDuration;
        [Range(0.1f, 1f)] [SerializeField] private float _repeatMultiplier;
        public float Duration => _baseDuration;
        public float RepeatMultiplier => _repeatMultiplier;

        private float _startTime;
        private float _endTime;

        public bool Check(float t)
        {
            return t >= _endTime;
        }
        
        public void Consume(float t, float speed = 1, float baseTimeMod = 0)
        {
            _startTime = t;
            _endTime = _startTime + (Duration + baseTimeMod) / speed;
        }

        public float GetPercentage(float t)
        {
            return Mathf.InverseLerp(_startTime, _endTime, t);
        }
        
        public void Reset(float t)
        {
            _startTime = t;
            _endTime = t + Duration;
        }
    }
}
