using Gameplay;
using Gameplay.Data;
using UI.Elements;
using UnityEngine;

namespace UI.Components
{
    public enum UIStat
    {
        Life,
        Barrier,
    }
    
    public class PlayerStatsComponent : MonoBehaviour
    {
        [SerializeField] private ProgressBar _healthBar;
        [SerializeField] private ProgressBar _barrierBar;
        [SerializeField] private ProgressBar _experienceBar;

        private void Awake()
        {
            _experienceBar.Set(0);
        }

        public void SetObservables(Player player)
        {
            IEntity entity = player;
            entity.HitPool.LifePercentage += f => _healthBar.Set(f);
            entity.HitPool.BarrierPercentage += _barrierBar.Set;
            player.ExperiencePercentage += _experienceBar.Set;
        }
    }
}
