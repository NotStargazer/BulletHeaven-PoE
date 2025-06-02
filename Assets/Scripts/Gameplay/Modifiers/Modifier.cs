using System;

namespace Gameplay.Modifiers
{
    public abstract class Modifier
    {
        public abstract bool IsPersistent { get; }
        public abstract Stat[] Calculate(IEntity entity, Skill skill = null);
        public abstract string GetEditorDescription();
    }
}
