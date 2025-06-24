using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Visuals.VisualEffects
{
    public struct LightningBoltData
    {
        public Vector3 Source;
        public Vector3 Target;
        public bool HasTarget;
    }
    
    public class LightningBoltVFX : VisualEffect<LightningBoltData>
    {
        private static readonly int SEED = Shader.PropertyToID("_Seed");
        private static readonly int FREQUENCY = Shader.PropertyToID("_Frequency");

        [SerializeField] private Transform _transform;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private AnimationClip _animationTarget;
        [SerializeField] private AnimationClip _animationNoTarget;
        
        private AnimationClip _animation;

        protected override float Duration => _animation.length;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        protected override void OnPlay(LightningBoltData data)
        {
            gameObject.SetActive(true);
            var properties = new MaterialPropertyBlock();
            if (_renderer.HasPropertyBlock())
            {
                _renderer.GetPropertyBlock(properties);
            }
            _animation = data.HasTarget ? _animationTarget : _animationNoTarget;
            properties.SetInteger(SEED, Random.Range(-255, 256));
            var middle = (data.Source + data.Target) / 2;
            var length = Vector3.Distance(data.Source, data.Target);
            var heightDelta = data.Source.y - data.Target.y;
            var angle = Mathf.Asin(heightDelta / length) * Mathf.Rad2Deg;
            var direction = (data.Source.ToXZ() - data.Target.ToXZ()).normalized;
            var spin = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            properties.SetFloat(FREQUENCY, length / 5);
            _transform.localPosition = middle;
            _transform.localScale = new Vector3(1, length);
            _transform.rotation = Quaternion.Euler(90 - angle, spin, 0);
            Debug.DrawLine(data.Source, data.Target, Color.red, 1);
            _renderer.SetPropertyBlock(properties);
        }

        protected override void OnFinish()
        {
            gameObject.SetActive(false);
            Return(this);
        }

        protected override void Tick(float percentage, float time)
        {
            _animation.SampleAnimation(gameObject, time);
        }
    }
}
