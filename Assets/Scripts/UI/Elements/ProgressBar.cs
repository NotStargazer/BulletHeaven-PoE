using UnityEngine;

namespace UI.Elements
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private AnimationClip _animationClip;
        
        public void Set(float d)
        {
            _animationClip.SampleAnimation(gameObject, d);
        }
    }
}
