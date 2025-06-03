using System.Collections.Generic;
using Gameplay.Data;
using UnityEngine;
using Utility;

namespace Gameplay.Modifiers
{
    public class StatTracker : MonoBehaviour
    {
        [SerializeField] private InterfaceRef<IEntity> _entity;
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
            var stats = GetStats(skill, scalers);
            return new DamageSnapshot(Stat.CalculateStats(baseDamage, stats));
        }

        public void CalculateResistance(DamageSnapshot damage)
        {
            if (damage.Physical.Damage > 0)
            {
                var stats = GetStats(statTypes: StatType.MultiType(BaseType.Resistance, DamageType.Generic, DamageType.Physical));
                var physicalDamage = CalculateResistance(0, stats);
                damage.Physical = new FlatDamage(DamageType.Physical, physicalDamage);
            }
            if (damage.Fire.Damage > 0)
            {
                var stats = GetStats(statTypes: StatType.MultiType(BaseType.Resistance, DamageType.Generic, DamageType.Elemental, DamageType.Fire));
                var fireDamage = CalculateResistance(0, stats);
                damage.Fire = new FlatDamage(DamageType.Physical, fireDamage);
            }
            if (damage.Cold.Damage > 0)
            {
                var stats = GetStats(statTypes: StatType.MultiType(BaseType.Resistance, DamageType.Generic, DamageType.Elemental, DamageType.Cold));
                var coldDamage = CalculateResistance(0, stats);
                damage.Cold = new FlatDamage(DamageType.Physical, coldDamage);
            }
            if (damage.Lightning.Damage > 0)
            {
                var stats = GetStats(statTypes: StatType.MultiType(BaseType.Resistance, DamageType.Generic, DamageType.Elemental, DamageType.Lightning));
                var lightningDamage = CalculateResistance(0, stats);
                damage.Lightning = new FlatDamage(DamageType.Physical, lightningDamage);
            }
        }

        private float CalculateResistance(float damage, Stat[] stats)
        {
            var resist = Stat.CalculateStats(0, stats);
            var cappedResist = Mathf.Min(resist, 75) / 100;
            var adjustedResist = 1 - cappedResist;
            return damage * adjustedResist;
        }
    }
}
