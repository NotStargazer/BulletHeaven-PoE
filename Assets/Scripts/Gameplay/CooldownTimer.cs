using System;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public struct CooldownTimer
    {
        [SerializeField] private float _baseDuration;
        public float Duration => _baseDuration;

        private float _startTime;
        private float _endTime;

        public bool Check(float t, float speed = 1, float baseTimeMod = 0)
        {
            if (t >= _endTime)
            {
                _startTime = _endTime;
                _endTime += (Duration + baseTimeMod) / speed;
                return true;
            }

            return false;
        }

        public float GetPercentage(float t)
        {
            return Mathf.InverseLerp(_startTime, _endTime, t);
        }
        
        public void Reset(float t)
        {
            _startTime = t;
            _endTime += t + Duration;
        }
    }
}
