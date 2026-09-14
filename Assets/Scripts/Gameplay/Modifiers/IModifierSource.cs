using System;

namespace Gameplay.Modifiers
{
    public interface IModifierSource
    {
        public Action<Modifier> AddModifier { get; set; }
        public Action<Modifier> RemoveModifier { get; set; }
    }
}
