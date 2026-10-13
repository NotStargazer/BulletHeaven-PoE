using UnityEngine;

namespace Visuals.VisualEffects
{
    public struct SlashData
    {
        public Vector3 Center;
        public Vector3 Forward;
        public float Radius;
    }
    
    public class SlashVFX : VisualEffect<SlashData>
    {
        [SerializeField] private float _sizeMultiplier = 0.5f;
        [SerializeField] private ParticleSystem _vfx;

        protected override float Duration => _vfx.main.duration;
        private void Awake()
        {
            gameObject.SetActive(false);
        }
        
        protected override void OnPlay(SlashData data)
        {
            var size = data.Radius * _sizeMultiplier;
            transform.position = data.Center;
            transform.right = data.Forward;
            transform.localScale = new Vector3(size, 1, size);
            gameObject.SetActive(true);
            _vfx.Play(true);
        }

        protected override void OnFinish()
        {
            gameObject.SetActive(false);
            Return(this);
        }

        protected override void Tick(float percentage, float time) { }
    }
}