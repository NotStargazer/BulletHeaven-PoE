using System.Collections.Generic;
using Gameplay.Data;

namespace Gameplay.Modifiers
{
    public class ModifierCollection
    {
        private Player _player;
        private List<IModifierSource> _sources;
        private List<Stat> _returnStats;

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
