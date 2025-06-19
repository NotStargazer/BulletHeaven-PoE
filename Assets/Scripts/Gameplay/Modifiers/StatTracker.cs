using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Gameplay.Data;
using UnityEngine;
using Utility;

namespace Gameplay.Modifiers
{
    public class StatTracker : MonoBehaviour
    {
        [SerializeField] private InterfaceRef<IEntity> _entity;
        [SerializeField] private SerializedDictionary<StatType, float> _baseStats;
        
        private List<IModifierSource> _sources;
        private Dictionary<StatType, Stat> _stats;
        private List<Modifier> _modifiers;
        private List<Stat> _outStats;

        public void Awake()
        {
            _sources = new List<IModifierSource>();
            _stats = new Dictionary<StatType, Stat>();
            _modifiers = new List<Modifier>();
            _outStats = new List<Stat>();
            
            Recalculate();
        }

        public void AddSource(IModifierSource source)
        {
            _sources.Add(source);
        }

        public void Recalculate()
        {
            _stats.Clear();
            _modifiers.Clear();
            foreach (var stat in _baseStats)
            {
                if (!_stats.TryAdd(stat.Key, new Stat(stat.Key, stat.Value, 0)))
                {
                    _stats[stat.Key] += new Stat(stat.Key, stat.Value, 0);
                } 
            }
            foreach (var source in _sources)
            {
                var mods = source.GetModifiers();
                foreach (var mod in mods)
                {
                    if (mod == null)
                    {
                        continue;
                    }

                    if (mod.IsConditional)
                    {
                        _modifiers.Add(mod);
                    }
                    else
                    {
                        var stats = mod.Calculate(_entity.I);
                        foreach (var stat in stats)
                        {
                            if (!_stats.TryAdd(stat.StatType, stat))
                            {
                                _stats[stat.StatType] += stat;
                            } 
                        }
                    }
                }
            }
        }

        public Stat[] GetStats(Skill skill = null, params StatType[] statTypes)
        {
            foreach (var statType in statTypes)
            {
                if (_stats.TryGetValue(statType, out var stat))
                {
                    _outStats.Add(stat);
                }
            }
            
            foreach (var modifier in _modifiers)
            {
                var modStats = modifier.Calculate(_entity.I, skill);
                foreach (var modStat in modStats)
                {
                    foreach (var statType in statTypes)
                    {
                        if (modStat.StatType == statType)
                        {
                            _outStats.Add(modStat);
                            break;
                        }
                    }
                }
            }

            var outStats = _outStats.ToArray();
            _outStats.Clear();
            return outStats;
        }

        public DamageSnapshot CalculateDamage(FlatDamage baseDamage, Skill skill, params StatType[] scalers)
        {
            Array.Resize(ref scalers, scalers.Length + 1);
            scalers[^1] = new StatType(BaseType.Damage, baseDamage.DamageType);
            var stats = GetStats(skill, scalers);
            return new DamageSnapshot(Stat.CalculateStats(baseDamage, stats));
        }

        public void CalculateResistances(DamageSnapshot damage)
        {
            damage.Physical = CalculateResistance(damage.Physical);
            damage.Fire = CalculateResistance(damage.Fire);
            damage.Cold = CalculateResistance(damage.Cold);
            damage.Lightning = CalculateResistance(damage.Lightning);
        }

        private float CalculateResistance(FlatDamage flatDamage)
        {
            if (flatDamage.Damage > 0)
            {
                var stats = GetStats(statTypes: new StatType(BaseType.Resistance, flatDamage.DamageType));
                var resist = Stat.CalculateStats(stats);
                var cappedResist = Mathf.Min(resist, 75) / 100;
                var adjustedResist = 1 - cappedResist;
                return flatDamage * adjustedResist;
            }

            return 0;
        }
    }
}
