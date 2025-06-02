using System.Collections.Generic;
using Gameplay.Data;

namespace Gameplay.Modifiers
{
    public class StatTracker
    {
        private readonly IEntity _entity;
        private readonly List<IModifierSource> _sources;
        private readonly Dictionary<StatType, Stat> _stats;
        private readonly List<Modifier> _modifiers;

        public StatTracker(IEntity entity)
        {
            _entity = entity;
            _sources = new List<IModifierSource>();
            _stats = new Dictionary<StatType, Stat>();
            _modifiers = new List<Modifier>();
        }

        public void AddSource(IModifierSource source)
        {
            _sources.Add(source);
        }

        public void Recalculate()
        {
            _stats.Clear();
            _modifiers.Clear();
            foreach (var source in _sources)
            {
                var mods = source.GetModifiers();
                foreach (var mod in mods)
                {
                    if (mod == null)
                    {
                        continue;
                    }

                    if (mod.IsPersistent)
                    {
                        var stats = mod.Calculate(_entity);
                        foreach (var stat in stats)
                        {
                            if (!_stats.TryAdd(stat.StatType, stat))
                            {
                                _stats[stat.StatType] += stat;
                            } 
                        }
                    }
                    else
                    {
                        _modifiers.Add(mod);
                    }
                }
            }
        }

        private Stat[] GetStats(Skill skill, params StatType[] scalers)
        {
            foreach (var statType in scalers)
            {
                
            }
        }

        public DamageSnapshot CalculateDamage(FlatDamage baseDamage, Skill skill, params StatType[] scalers)
        {
            var stats = GetStats(skill, scalers);
            return new DamageSnapshot(Stat.CalculateStats(baseDamage, stats));
        }
    }
}
