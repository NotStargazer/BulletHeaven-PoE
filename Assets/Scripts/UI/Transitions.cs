using System;
using Global;
using UnityEngine;

namespace UI
{
    public class TransitionFunction : IDataBehaviour
    {
        private readonly Action<float> _callback;
        public float RemainingTime => Mathf.Max(_endTime - Time.time, 0);
        public bool Enabled { get; private set; }

        public TransitionFunction(Action<float> callback)
        {
            if (GlobalResources.HasInstance)
            {
                _callback = callback;
                
                GlobalResources.Instance.DataBehaviours.AddBehaviour(this);
            }
            else
            {
                throw new Exception("UI Elements must run in awake function.");
            }
        }

        private float _startValue;
        private float _targetValue;
        
        private float _startTime;
        private float _endTime;

        public bool Active => Time.time >= _startTime && Time.time <= _endTime;

        private Func<float, float> _easeFunc;
        private Action _completionCallback;

        
        public void StartTransition(float duration, float startValue, float targetValue,
            float delay = 0, Func<float, float> easeFunc = null, Action onComplete = null)
        {
            _completionCallback = onComplete;
            
            _startValue = startValue;
            _targetValue = targetValue;

            _startTime = Time.time + delay;
            _easeFunc = easeFunc;
            _endTime = Time.time + duration + delay;
            Enabled = true;
        }

        public void Update(float delatTime, int behaviourId)
        {
            if (!Active)
            {
                if (Time.time > _endTime)
                {
                    _callback(GetValue(1));
                    Enabled = false;
                    _completionCallback?.Invoke();
                }

                return;
            }

            _callback(GetValue());
        }

        private float GetValue(float? progressOverride = null)
        {
            var l = progressOverride ?? Mathf.InverseLerp(_startTime, _endTime, Time.time);
            var ease = l;
            
            if (_easeFunc != null)
            {
                ease = _easeFunc(l);
            }

            return Mathf.LerpUnclamped(_startValue, _targetValue, ease);
        }

        public void Cancel()
        {
            Enabled = false;
        }

        public void RunOnce(float i)
        {
            _callback(i);
        }
    }
}
