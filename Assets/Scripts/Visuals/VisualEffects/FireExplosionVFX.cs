using UnityEngine;

namespace Visuals.VisualEffects
{
    public struct FireExplosionData
    {
        public Vector3 Center;
        public float Radius;
    }
    
    public class FireExplosionVFX : VisualEffect<FireExplosionData>
    {
        private static readonly int SEED = Shader.PropertyToID("_Seed");
        [SerializeField] private Transform _transform;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private AnimationClip _animation;
        [SerializeField] private float _radiusOffset;

        private MaterialPropertyBlock _propertyBlock;
        protected override float Duration => _animation.length;
        
        private void Awake()
        {
            gameObject.SetActive(false);
            _propertyBlock = new MaterialPropertyBlock();
        }
        protected override void OnPlay(FireExplosionData data)
        {
            var diameter = (_radiusOffset + data.Radius) * 2;
            _propertyBlock.SetInteger(SEED, Random.Range(-255, 256));
            _transform.localPosition = data.Center;
            _transform.localScale = new Vector3(diameter, diameter);
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