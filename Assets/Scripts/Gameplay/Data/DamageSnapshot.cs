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
        public FlatDamage Physical { get; set; }
        public FlatDamage Fire { get; set; }
        public FlatDamage Cold { get; set; }
        public FlatDamage Lightning { get; set; }

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
    }
}
