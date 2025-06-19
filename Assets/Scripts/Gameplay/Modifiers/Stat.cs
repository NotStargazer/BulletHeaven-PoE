using System;
using Gameplay.Data;
using UnityEngine;

namespace Gameplay.Modifiers
{
    [Serializable]
    public struct Stat
    {
        public static float CalculateStats(params Stat[] stats) => CalculateStats(0f, stats);
        public static FlatDamage CalculateStats(FlatDamage baseDamage, params Stat[] stats)
        {
            return baseDamage.Damage = CalculateStats(baseDamage.Damage, stats);
        }
        public static float CalculateStats(float baseStat, params Stat[] stats)
        {
            var result = new Stat();

            foreach (var stat in stats)
            {
                result += stat;
            }

            if (result._fixed)
            {
                baseStat = result._fixedStat;
                return baseStat;
            }

            baseStat += result._flat;
            baseStat *= result._increase + 1;
            baseStat *= result._multiplier + 1;

            return baseStat;
        }
        
        [SerializeField] private StatType _statType;

        [SerializeField] private float _flat;
        [SerializeField] private float _increase;
        [SerializeField] private float _multiplier;
        [SerializeField] private bool _fixed;
        [SerializeField] private float _fixedStat;

        public StatType StatType => _statType;

        public Stat(StatType statType, float flat = 0, float increase = 0, float multiplier = 0)
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
            _multiplier = 0;
            _fixed = true;
            _fixedStat = fixedStat;
        }

        public string GetEditorDescription(string append = "")
        {
            var returnString = new string[3];

            if (_fixed)
            {
                return $"{_statType.EditorStatName} is {_fixedStat}";
            }
            
            if (_flat != 0)
            {
                var direction = _flat > 0 ? "+" : "-";
                returnString[0] = $"{direction}{_flat} to {_statType.EditorStatName}{append}";
            }
            if (_increase != 0)
            {
                var direction = _increase > 0 ? "increased" : "decreased";
                returnString[1] = $"{_increase}% {direction} {_statType.EditorStatName}{append}";
            }

            if (_multiplier != 0)
            {
                var direction = _flat > 0 ? "more" : "less";
                returnString[2] = $"{Mathf.RoundToInt(_multiplier * 100)}% {direction} {_statType.EditorStatName}{append}";
            }

            for (var index = 0; index < returnString.Length - 1; index++)
            {
                if (!string.IsNullOrEmpty(returnString[index]))
                {
                    returnString[index] += "\n";
                }
            }

            return string.Concat(returnString);
        }

        public static Stat operator +(Stat lhs, Stat rhs)
        {
            if (lhs._fixed && rhs._fixed)
            {
                return new Stat(lhs._statType, Mathf.Min(lhs._fixedStat, rhs._fixedStat));
            }

            if (lhs._fixed)
            {
                return new Stat(lhs._statType, lhs._fixedStat);
            }

            if (rhs._fixed)
            {
                return new Stat(rhs._statType, rhs._fixedStat);
            }

            return new Stat(lhs._statType,
                lhs._flat + rhs._flat,
                lhs._increase + rhs._increase,
                lhs._multiplier + rhs._multiplier);
        }
        public static Stat operator -(Stat lhs, Stat rhs)
        {
            if (lhs._fixed || rhs._fixed)
            {
                return new Stat(lhs._statType, lhs._fixedStat);
            }

            return new Stat(lhs._statType,
                lhs._flat - rhs._flat,
                lhs._increase - rhs._increase,
                lhs._multiplier - rhs._multiplier);
        }
    }
}
