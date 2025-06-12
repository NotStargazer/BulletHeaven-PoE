using System;
using UnityEngine;

namespace Graphics
{
    public struct VisualEffect
    {
        public Material Material;
        public AnimationClip AnimationClip;
    }
    
    public class VisualEffectAnimator : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        
        private AnimationClip _animationClip;
        private float _startTime;
        private float _endTime;

        private bool _enabled;
        private bool Active => Time.time >= _startTime && Time.time <= _endTime && _enabled;
        
        public void Play(VisualEffect effect)
        {
            _startTime = Time.time;
            _endTime = Time.time + effect.AnimationClip.length;
            _animationClip = effect.AnimationClip;
            _enabled = true;
        }

        private void Update()
        {
            if (Active)
            {
                _animationClip.SampleAnimation(gameObject, Time.time - _startTime);
            }
            else if (Time.time >= _endTime)
            {
                _enabled = false;
                Destroy(gameObject);
            }
        }
    }
}
