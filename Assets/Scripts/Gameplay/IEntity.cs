using Gameplay.Data;
using Gameplay.Modifiers;
using UnityEngine;

namespace Gameplay
{
    public interface IEntity
    {
        public Transform transform { get; }
        public StatTracker StatTracker { get; protected set; }
        public HitPool HitPool { get; protected set; }
        public SkillManager SkillManager { get; protected set; }
        
        public void ImplementComponents()
        {
            if (!transform.TryGetComponent<StatTracker>(out var statTracker))
            {
                StatTracker = transform.gameObject.AddComponent<StatTracker>();
            }
            else if (!StatTracker)
            {
                StatTracker = statTracker;
            }

            if (!transform.TryGetComponent<HitPool>(out var hitPool))
            {
                HitPool = transform.gameObject.AddComponent<HitPool>();
            }
            else if (!HitPool)
            {
                HitPool = hitPool;
            }

            if (!transform.TryGetComponent<SkillManager>(out var skillManager))
            {
                SkillManager = transform.gameObject.AddComponent<SkillManager>();
            }
            else if (!SkillManager)
            {
                SkillManager = skillManager;
            }
        }
    }
}
