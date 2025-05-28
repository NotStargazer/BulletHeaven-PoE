using System.Collections.Generic;

namespace Gameplay.Modifiers
{
    public interface IModifierSource
    {
        public IEnumerable<Modifier> GetModifiers();
    }
}
