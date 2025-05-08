using System;
using Gameplay.Data;
using UnityEngine;

namespace Gameplay.Modifiers
{
    [Serializable]
    public struct Stat
    {
        public static float CalculateStats(float baseDamage, params Stat[] stats)
        {
            var result = new Stat();

            foreach (var stat in stats)
            {
                result += stat;
            }

            if (result._fixed)
            {
                return result._fixedStat;
            }

            baseDamage += result._flat;
            baseDamage *= result._increase;
            baseDamage *= result._multiplier;

            return baseDamage;
        }
        
        [SerializeField] private StatType _statType;

        [SerializeField] private float _flat;
        [SerializeField] private float _increase;
        [SerializeField] private float _multiplier;
        [SerializeField] private bool _fixed;
        [SerializeField] private float _fixedStat;

        public Stat(StatType statType, float flat = 0, float increase = 0, float multiplier = 1)
        {
            _statType = statType;
            
            _flat = flat;
            _increase = increase;
            _multiplier = multiplier;
            _fixed = false;
            _fixedStat = Mathf.Infinity;
        }

        public Stat(StatType statType, float fixedStat)
        {
            _statType = statType;
            
            _flat = 0;
            _increase = 0;
            _multiplier = 1;
            _fixed = true;
            _fixedStat = fixedStat;
        }

        public static Stat operator +(Stat lhs, Stat rhs)
        {
            if (lhs._fixed || rhs._fixed)
            {
                return new Stat(lhs._statType, Mathf.Min(lhs._flat, rhs._flat));
            }

            return new Stat(lhs._statType,
                lhs._flat + rhs._flat,
                lhs._increase + rhs._increase,
                lhs._multiplier * rhs._multiplier);
        }
    }
}
