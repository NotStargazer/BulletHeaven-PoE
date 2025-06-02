using Gameplay.Modifiers;
using UnityEngine;

namespace Gameplay
{
    public interface IEntity
    {
        public Transform Transform { get; }
        public StatTracker StatTracker { get; }
    }
}
