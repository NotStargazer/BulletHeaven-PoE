using System;
using Gameplay.Data;

namespace Gameplay.Modifiers
{
    public abstract class Modifier
    {
        private StatType _validTypes;
        public abstract bool IsConditional { get; }
        public abstract Stat[] Calculate(IEntity entity, Skill skill = null);
        public abstract string GetEditorDescription();
    }
}
