using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Gameplay.Data;
using Gameplay.Skills;
using UnityEngine;
using Utility;

namespace Gameplay.Modifiers
{
    public class StatTracker : MonoBehaviour
    {
        [SerializeField] private InterfaceRef<IEntity> _entity;
        [SerializeField] private SerializedDictionary<StatType, float> _baseStats;
        
        private byte _checkIteration;
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

        public List<Stat> GetStats(Skill skill = null, params StatType[] statTypes)
        {
            _checkIteration++;
            _outStats.Clear();
            GetStatsNoIterate(_outStats, skill, statTypes);
            return _outStats;
        }
        
        public void GetStatsNoIterate(List<Stat> stats, Skill skill = null, params StatType[] statTypes)
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
                        stats.Add(stat);
                    }
                });
                
                foreach (var modifier in _modifiers)
                {
                    if (modifier.ModCheckIteration == _checkIteration)
                    {
                        continue;
                    }
                    modifier.ModCheckIteration = _checkIteration;
                    var modStats = modifier.Calculate(_entity.I, skill);
                    if (modStats == null)
                    {
                        continue;
                    }
                    foreach (var modStat in modStats)
                    {
                        if (modStat.StatType == statType)
                        {
                            stats.Add(modStat);
                            break;
                        }
                    }
                }
            }
        }

        private float GetAndCalculate(Skill skill = null, params StatType[] statTypes)
        {
            GetStats(skill, statTypes);
            return Stat.CalculateStats(_outStats);
        }
        
        private float GetAndCalculateAsDiminishing(Skill skill = null, params StatType[] statTypes)
        {
            GetStats(skill, statTypes);
            return Stat.CalculateDiminishing(_outStats);
        }

        public DamageSnapshot CalculateDamage(FlatDamage baseDamage, Skill skill, params StatType[] scalers)
        {
            GetStats(skill, scalers);
            return new DamageSnapshot(Stat.CalculateStats(baseDamage, _outStats));
        }
        
        public PenetrationSnapshot GetResistancePenetration(Skill skill)
        {
            var physical = GetAndCalculate(skill, new StatType(BaseType.Penetration, DamageType.Physical));
            var fire = GetAndCalculate(skill, new StatType(BaseType.Penetration, DamageType.Fire));
            var cold = GetAndCalculate(skill, new StatType(BaseType.Penetration, DamageType.Cold));
            var lightning = GetAndCalculate(skill, new StatType(BaseType.Penetration, DamageType.Lightning));
            //TODO Flag Mods for ignoring penetration?
            return new PenetrationSnapshot(physical, fire, cold, lightning);
        }

        public DamageSnapshot CalculateResistances(DamageSnapshot damage, PenetrationSnapshot penetration)
        {
            damage.Physical = CalculateResistance(damage.Physical, penetration.Physical);
            damage.Fire = CalculateResistance(damage.Fire, penetration.Fire);
            damage.Cold = CalculateResistance(damage.Cold, penetration.Cold);
            damage.Lightning = CalculateResistance(damage.Lightning, penetration.Lightning);

            return damage;
        }
        
        public SpeedSnapshot GetSkillSpeed(Skill skill, SkillTags skillTags)
        {
            var isSpell = (skillTags & SkillTags.Spell) == SkillTags.Spell;
            var isAttack = (skillTags & SkillTags.Attack) == SkillTags.Attack;
                          GetStats(skill, 
                isSpell ? new StatType(BaseType.Speed, ActionType.Cast) : null,
                isAttack ? new StatType(BaseType.Speed, ActionType.Attack) : null);
            return new SpeedSnapshot(_outStats);
        }

        public float GetMovementSpeed()
        {
            var speed = GetStats(statTypes: new StatType(BaseType.Speed, ActionType.Move));
            return Stat.CalculateStats(speed);
        }
        
        public HitPoolSnapshot GetHitPoolStats()
        {
            return new HitPoolSnapshot(
            GetAndCalculate(statTypes: new StatType(BaseType.Resource, ResourceType.Life)),
            GetAndCalculate(statTypes: new StatType(BaseType.Defence, DefenceType.Barrier)),
            GetAndCalculate(statTypes: new StatType(BaseType.ResourceRecovery, ResourceType.Life)),
            GetAndCalculateAsDiminishing(statTypes: new StatType(BaseType.Barrier, BarrierStat.StartTime)),
            GetAndCalculateAsDiminishing(statTypes: new StatType(BaseType.Barrier, BarrierStat.RegenRate)));
        }

        private float CalculateResistance(FlatDamage flatDamage, float? penetration)
        {
            if (flatDamage.Damage > 0)
            {
                var stats = GetStats(statTypes: new StatType(BaseType.Resistance, flatDamage.DamageType));
                var resist = Stat.CalculateStats(stats);
                if (penetration.HasValue)
                {
                    resist -= penetration.Value;
                }
                else
                {
                    resist = 0;
                }
                var cappedResist = Mathf.Min(resist, 75) / 100;
                var adjustedResist = 1 - cappedResist;
                return flatDamage * adjustedResist;
            }

            return 0;
        }

        public ProjectileSnapshot GetProjectileStats(int count, float speed, float range, int pierce, Skill skill = null)
        {
            return new ProjectileSnapshot(
                Stat.CalculateStats(count,  GetStats(skill, new StatType(BaseType.Projectile, ProjectileStat.Count))),
                Stat.CalculateStats(speed,  GetStats(skill, new StatType(BaseType.Projectile, ProjectileStat.Speed))),
                Stat.CalculateStats(range,  GetStats(skill, new StatType(BaseType.Projectile, ProjectileStat.Range))),
                Stat.CalculateStats(pierce, GetStats(skill, new StatType(BaseType.Projectile, ProjectileStat.Pierce)))
            );
        }
    }
}
