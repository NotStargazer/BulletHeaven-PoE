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
            
            foreach (var stat in _baseStats)
            {
                stat.Key.Split(splitType =>
                {
                    if (!_stats.TryAdd(splitType, new Stat(splitType, stat.Value, 0)))
                    {
                        _stats[splitType] += new Stat(splitType, stat.Value, 0);
                    } 
                });
            }
        }

        public void AddSource(IModifierSource source)
        {
            _sources.Add(source);

            source.AddModifier += AddModifer;
            source.RemoveModifier += RemoveModifier;
        }

        private void AddModifer(Modifier mod)
        {
            if (mod == null)
            {
                return;
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
                    stat.StatType.Split(splitType =>
                    {
                        if (!_stats.TryAdd(splitType, stat))
                        {
                            _stats[splitType] += stat;
                        } 
                    });
                }
            }
        }

        private void RemoveModifier(Modifier mod)
        {
            if (mod == null)
            {
                return;
            }
            
            if (mod.IsConditional)
            {
                _modifiers.Remove(mod);
            }
            else
            {
                var stats = mod.Calculate(_entity.I);
                foreach (var stat in stats)
                {
                    stat.StatType.Split(splitType =>
                    {
                        if (!_stats.ContainsKey(splitType))
                        {
                            _stats[splitType] -= stat;
                        } 
                    });
                }
            }
        }

        public Stat[] GetStats(Skill skill = null, params StatType[] statTypes)
        {
            foreach (var statType in statTypes)
            {
                if (statType is null)
                {
                    continue;
                }
                
                statType.Split(splitType =>
                {
                    if (_stats.TryGetValue(splitType, out var stat))
                    {
                        _outStats.Add(stat);
                    }
                });
                
                foreach (var modifier in _modifiers)
                {
                    var modStats = modifier.Calculate(_entity.I, skill);
                    if (modStats == null)
                    {
                        continue;
                    }
                    foreach (var modStat in modStats)
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

        public DamageSnapshot CalculateResistances(DamageSnapshot damage)
        {
            damage.Physical = CalculateResistance(damage.Physical);
            damage.Fire = CalculateResistance(damage.Fire);
            damage.Cold = CalculateResistance(damage.Cold);
            damage.Lightning = CalculateResistance(damage.Lightning);

            return damage;
        }
        
        public SpeedSnapshot GetSkillSpeed(Skill skill, SkillTags skillTags)
        {
            var isSpell = (skillTags & SkillTags.Spell) == SkillTags.Spell;
            var isAttack = (skillTags & SkillTags.Attack) == SkillTags.Attack;
                          var stats = GetStats(skill, 
                isSpell ? new StatType(BaseType.Speed, ActionType.Cast) : null,
                isAttack ? new StatType(BaseType.Speed, ActionType.Attack) : null);
            return new SpeedSnapshot(stats);
        }

        public float GetMovementSpeed()
        {
            var speed = GetStats(statTypes: new StatType(BaseType.Speed, ActionType.Move));
            return Stat.CalculateStats(speed);
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
