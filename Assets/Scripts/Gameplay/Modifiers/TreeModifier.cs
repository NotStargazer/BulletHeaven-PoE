using System;
using Global;
using UnityEngine;

namespace Gameplay.Modifiers
{
    [Serializable]
    public class TreeModifier : StatModifier, IRegistrableData
    {
        [SerializeField] private Texture _treeIcon;
        
        public object GetInstance()
        {
            return (TreeModifier)MemberwiseClone();
        }
    }
}
