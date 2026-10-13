using Gameplay.Data;
using Gameplay.Modifiers;
using UnityEngine;

namespace Gameplay
{
    public interface IEntity
    {
        public Transform Transform { get; }
        public StatTracker StatTracker { get; protected set; }
        public HitPool HitPool { get; protected set; }
        public SkillManager SkillManager { get; protected set; }
        
        public void ImplementComponents()
        {
            if (!Transform.TryGetComponent<StatTracker>(out var statTracker))
            {
                StatTracker = Transform.gameObject.AddComponent<StatTracker>();
            }
            else if (!StatTracker)
            {
                StatTracker = statTracker;
            }

            if (!Transform.TryGetComponent<HitPool>(out var hitPool))
            {
                HitPool = Transform.gameObject.AddComponent<HitPool>();
            }
            else if (!HitPool)
            {
                HitPool = hitPool;
            }

            if (!Transform.TryGetComponent<SkillManager>(out var skillManager))
            {
                SkillManager = Transform.gameObject.AddComponent<SkillManager>();
            }
            else if (!SkillManager)
            {
                SkillManager = skillManager;
            }
        }
    }
}
