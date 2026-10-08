using System;
using System.Collections.Generic;
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
        public byte ModCheckIteration { get; set; }
        
        private StatType _validTypes;
        public abstract bool IsConditional { get; }
        public abstract IReadOnlyCollection<Stat> Calculate(IEntity entity, Skill skill = null);
        public abstract string GetDescription();
        protected virtual void Construct(){}
        
        public object GetInstance()
        {
            var clone = (Modifier)MemberwiseClone();
            clone.Construct();
            return clone;
        }
    }
}
