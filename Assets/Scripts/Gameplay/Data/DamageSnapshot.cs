using System;

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

        public static implicit operator float(FlatDamage damage) => damage.Damage;
        public static implicit operator FlatDamage(float damage) => new(0, damage);
        
        public DamageType DamageType;
        public float Damage;
    }
    
    public struct DamageSnapshot
    {
        public FlatDamage Physical
        {
            get => new(DamageType.Physical, _physical);
            set => _physical = value.Damage;
        }
        public FlatDamage Fire
        {
            get => new(DamageType.Fire, _fire);
            set => _fire = value.Damage;
        }
        public FlatDamage Cold
        {
            get => new(DamageType.Cold, _cold);
            set => _cold = value.Damage;
        }
        public FlatDamage Lightning
        {
            get => new(DamageType.Lightning, _lightning);
            set => _lightning = value.Damage;
        }

        private float _physical;
        private float _fire;
        private float _cold;
        private float _lightning;
        
        public static implicit operator float(DamageSnapshot snapshot)
        {
            return snapshot._physical + snapshot._fire + snapshot._cold + snapshot._lightning;
        }

        public DamageSnapshot(params FlatDamage[] damage)
        {
            _physical = 0;
            _fire = 0;
            _cold = 0;
            _lightning = 0;
            foreach (var flatDamage in damage)
            {
                if ((flatDamage.DamageType & DamageType.Physical) == DamageType.Physical)
                {
                    _physical = flatDamage.Damage;
                }
                if ((flatDamage.DamageType & DamageType.Fire) == DamageType.Fire)
                {
                    _fire = flatDamage.Damage;
                }
                if ((flatDamage.DamageType & DamageType.Cold) == DamageType.Cold)
                {
                    _cold = flatDamage.Damage;
                }
                if ((flatDamage.DamageType & DamageType.Lightning) == DamageType.Lightning)
                {
                    _lightning = flatDamage.Damage;
                }
            }
        }
    }
    
    public struct PenetrationSnapshot
    {
        public readonly float? Physical;
        public readonly float? Fire;
        public readonly float? Cold;
        public readonly float? Lightning;

        public PenetrationSnapshot(float? physical, float? fire, float? cold, float? lightning)
        {
            Physical = physical;
            Fire = fire;
            Cold = cold;
            Lightning = lightning;
        }
    }
}
