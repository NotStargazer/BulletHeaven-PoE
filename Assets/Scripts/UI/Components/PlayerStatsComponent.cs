using UI.Elements;
using UnityEngine;

namespace UI.Components
{
    public class PlayerStatsComponent : MonoBehaviour
    {
        [SerializeField] private ProgressBar _healthBar;
        [SerializeField] private ProgressBar _experienceBar;

        public void SetHealth(float p)
        {
            _healthBar.Set(p);
        }

        public void Experience(float p)
        {
            _experienceBar.Set(p);
        }
    }
}
