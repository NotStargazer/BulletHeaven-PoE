using System;
using Global;
using UnityEngine;

namespace Gameplay.Modifiers
{
    public enum TreeAlignment
    {
        Red,
        Green,
        Blue,
    }
    
    [Serializable]
    public class TreeModifier : StatModifier, IRegistrableData
    {
        [SerializeField] private TreeAlignment _alignment;
        [Range(0, 100)] [SerializeField] private float _minDistance;
        
        public object GetInstance()
        {
            var treeMod = (TreeModifier)MemberwiseClone();
            return treeMod;
        }
    }
}
