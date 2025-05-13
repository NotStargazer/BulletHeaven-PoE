using System;

namespace Gameplay.Modifiers
{
    public abstract class Modifier
    {
        public abstract Stat[] Calculate(Player player, Skill skill);
        public abstract string GetEditorDescription();
    }
}
