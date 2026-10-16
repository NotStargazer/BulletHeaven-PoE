using UnityEngine;

namespace Visuals.VisualEffects
{
    public struct HitSparkData
    {
        public Vector3 Center;
    }
    
    public class HitVFX : VisualEffect<HitSparkData>
    {
        [SerializeField] private ParticleSystem _vfx;

        protected override float Duration => _vfx.main.duration;
        private void Awake()
        {
            gameObject.SetActive(false);
        }
        
        protected override void OnPlay(HitSparkData data)
        {
            transform.position = data.Center;
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