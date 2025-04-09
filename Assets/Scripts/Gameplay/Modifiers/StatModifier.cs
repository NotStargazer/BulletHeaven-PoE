using System;
using UnityEngine;

namespace Gameplay.Modifiers
{
    [Serializable]
    public class StatModifier : Modifier
    {
        [SerializeField] private Stat _stat;
        
        public override Stat Calculate(Player player)
        {
            return _stat;
        }
    }
}
