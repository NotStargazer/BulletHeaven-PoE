namespace Visuals.VisualEffects
{
    public struct FireExplosionData
    {
        public float Radius;
    }
    
    public class FireExplosionVFX : VisualEffect<FireExplosionData>
    {
        protected override float Duration { get; }
        protected override void OnPlay(FireExplosionData data)
        {
            
        }

        protected override void OnFinish()
        {
        }

        protected override void Tick(float percentage, float time)
        {
        }
    }
}