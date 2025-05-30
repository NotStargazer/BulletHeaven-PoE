using System.Collections.Generic;
using Gameplay.Data;

namespace Gameplay.Modifiers
{
    public class ModifierCollection
    {
        private readonly Player _player;
        private readonly List<IModifierSource> _sources;
        private readonly List<Stat> _returnStats;

        public ModifierCollection(Player player)
        {
            _player = player;
            _sources = new List<IModifierSource>();
            _returnStats = new List<Stat>();
        }

        public void AddSource(IModifierSource source)
        {
            _sources.Add(source);
        }

        public Stat[] GetStats(Skill skill, params StatType[] statTypes)
        {
            _returnStats.Clear();
            foreach (var source in _sources)
            {
                var mods = source.GetModifiers();
                foreach (var mod in mods)
                {
                    if (mod == null)
                    {
                        continue;
                    }
                    var stats = mod.Calculate(_player, skill);
                    foreach (var stat in stats)
                    {
                        foreach (var statType in statTypes)
                        {
                            if (stat.StatType == statType)
                            {
                                _returnStats.Add(stat);
                                break;
                            }
                        }
                    }
                }
            }

            return _returnStats.ToArray();
        }
    }
}
