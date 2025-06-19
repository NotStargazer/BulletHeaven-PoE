using System;
using Gameplay;
using UnityEngine;

namespace Visuals
{
    public abstract class VisualEffect<T> : MonoBehaviour, IPoolInstance<VisualEffect<T>>
    {
        private float _startTime;
        private float _endTime;

        private bool _enabled;
        private bool Active => Time.time >= _startTime && Time.time <= _endTime && _enabled;
        
        public void Play(T data)
        {
            OnPlay(data);
            _enabled = true;
            _startTime = Time.time;
            _endTime = Time.time + Duration;
        }

        protected abstract float Duration { get; }
        protected abstract void OnPlay(T data);
        protected abstract void OnFinish();
        protected abstract void Tick(float percentage, float time);

        private void Update()
        {
            if (Active)
            {
                Tick(Mathf.InverseLerp(_startTime, _endTime, Time.time), Time.time - _startTime);
            }
            else if (Time.time >= _endTime && _enabled)
            {
                Tick(1, Duration);
                OnFinish();
                _enabled = false;
            }
        }

        public void OnRent() { }

        public Action<VisualEffect<T>> Return { get; set; }
    }
}
