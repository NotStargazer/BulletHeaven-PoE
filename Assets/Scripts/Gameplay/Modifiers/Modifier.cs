using System;
using Gameplay.Data;
using Global;
using UnityEngine;

namespace Gameplay.Modifiers
{
    [Serializable]
    public abstract class Modifier : IRegistrableData
    {
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;
        
        private StatType _validTypes;
        public abstract bool IsConditional { get; }
        public abstract Stat[] Calculate(IEntity entity, Skill skill = null);
        public abstract string GetDescription();
        
        public object GetInstance()
        {
            return (Modifier)MemberwiseClone();
        }
    }
}
