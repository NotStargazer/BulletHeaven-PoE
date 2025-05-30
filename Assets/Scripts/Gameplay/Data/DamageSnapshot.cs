using System;
using Gameplay.Modifiers;

namespace Gameplay.Data
{
    [Serializable]
    public struct FlatDamage
    {
        public FlatDamage(DamageType damageType, float damage)
        {
            DamageType = damageType;
            Damage = damage;
        }
        
        public DamageType DamageType;
        public float Damage;

        public FlatDamage Set(FlatDamage damage)
        {
            return new FlatDamage(DamageType, damage.Damage);
        }
    }
    
    public struct DamageSnapshot
    {
        public FlatDamage Physical { get; private set; }
        public FlatDamage Fire { get; private set; }
        public FlatDamage Cold { get; private set; }
        public FlatDamage Lightning { get; private set; }

        public DamageSnapshot(params FlatDamage[] damage)
        {
            Physical = new FlatDamage(DamageType.Physical, 0);
            Fire = new FlatDamage(DamageType.Fire, 0);
            Cold = new FlatDamage(DamageType.Cold, 0);
            Lightning = new FlatDamage(DamageType.Lightning, 0);
            foreach (var flatDamage in damage)
            {
                switch (flatDamage.DamageType)
                {
                    case DamageType.Generic:
                        Physical = Physical.Set(flatDamage);
                        Fire = Fire.Set(flatDamage);
                        Cold = Cold.Set(flatDamage);
                        Lightning = Lightning.Set(flatDamage);
                        break;
                    case DamageType.Physical:
                        Physical = Physical.Set(flatDamage);
                        break;
                    case DamageType.Fire:
                        Fire = Fire.Set(flatDamage);
                        break;
                    case DamageType.Cold:
                        Cold = Cold.Set(flatDamage);
                        break;
                    case DamageType.Lightning:
                        Lightning = Lightning.Set(flatDamage);
                        break;
                    case DamageType.Elemental:
                        Fire = Fire.Set(flatDamage);
                        Cold = Cold.Set(flatDamage);
                        Lightning = Lightning.Set(flatDamage);
                        break;
                }
            }
        }

        public static DamageSnapshot Modify(DamageSnapshot snapshot, params Stat[] stats)
        {
            foreach (var stat in stats)
            {
                if (stat.StatType == new StatType(BaseType.Damage, DamageType.Generic))
                {
                    snapshot.Physical = Stat.CalculateStats(snapshot.Physical, stat);
                    snapshot.Fire = Stat.CalculateStats(snapshot.Fire, stat);
                    snapshot.Cold = Stat.CalculateStats(snapshot.Cold, stat);
                    snapshot.Lightning = Stat.CalculateStats(snapshot.Lightning, stat);
                }
                if (stat.StatType == new StatType(BaseType.Damage, DamageType.Elemental))
                {
                    snapshot.Fire = Stat.CalculateStats(snapshot.Fire, stat);
                    snapshot.Cold = Stat.CalculateStats(snapshot.Cold, stat);
                    snapshot.Lightning = Stat.CalculateStats(snapshot.Lightning, stat);
                }
                if (stat.StatType == new StatType(BaseType.Damage, DamageType.Physical))
                {
                    snapshot.Physical = Stat.CalculateStats(snapshot.Physical, stat);
                }
                if (stat.StatType == new StatType(BaseType.Damage, DamageType.Fire))
                {
                    snapshot.Fire = Stat.CalculateStats(snapshot.Fire, stat);
                }
                if (stat.StatType == new StatType(BaseType.Damage, DamageType.Cold))
                {
                    snapshot.Cold = Stat.CalculateStats(snapshot.Cold, stat);
                }
                if (stat.StatType == new StatType(BaseType.Damage, DamageType.Lightning))
                {
                    snapshot.Lightning = Stat.CalculateStats(snapshot.Lightning, stat);
                }
            }

            return snapshot;
        }
    }
}