using System;
using UnityEngine;
using Visuals;
using Visuals.VisualEffects;

namespace Gameplay.Skills
{
    [Serializable]
    public class ChainLightning : Skill
    {
        [SerializeField] private LightningBoltVFX _lightningVFX;
        private InstancePool<VisualEffect<LightningBoltData>> _vfxPool;
        protected override void Construct()
        {
            _vfxPool = new InstancePool<VisualEffect<LightningBoltData>>(_lightningVFX, 10, 5, 30);
        }

        protected override void Use(IEntity entity, Support[] supports)
        {
            
        }
    }
}
