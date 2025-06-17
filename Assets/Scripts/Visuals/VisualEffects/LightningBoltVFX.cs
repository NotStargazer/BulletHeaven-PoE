using UnityEngine;
using Random = UnityEngine.Random;

namespace Visuals.VisualEffects
{
    public struct LightningBoltData
    {
        public Vector3 Source;
        public Vector3 Target;
    }
    
    public class LightningBoltVFX : VisualEffect<LightningBoltData>
    {
        private static readonly int SEED = Shader.PropertyToID("_Seed");
        private static readonly int FREQUENCY = Shader.PropertyToID("_Frequency");

        [SerializeField] private Transform _transform;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private AnimationClip _animation;

        protected override float Duration => _animation.length;

        protected override void OnPlay(LightningBoltData data)
        {
            gameObject.SetActive(true);
            var properties = new MaterialPropertyBlock();
            if (_renderer.HasPropertyBlock())
            {
                _renderer.GetPropertyBlock(properties);
            }
            properties.SetInteger(SEED, Random.Range(int.MinValue + 1, int.MaxValue));
            _renderer.SetPropertyBlock(properties);
            var middle = (data.Source + data.Target) / 2;
            var length = Vector3.Distance(data.Source, data.Target);
            var direction = data.Source - data.Target;
            var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            properties.SetFloat(FREQUENCY, length / 2);
            _transform.localPosition = middle;
            _transform.localScale = new Vector3(0, length);
            _transform.localRotation = Quaternion.Euler(90, 0, angle);
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
