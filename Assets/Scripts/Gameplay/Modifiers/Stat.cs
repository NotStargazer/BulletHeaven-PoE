using System;
using Gameplay.Data;
using UI.Localization;
using UnityEngine;
using Utility.Attributes;

namespace Gameplay.Modifiers
{
    public enum StatFactor
    {
        Flat,
        Increased,
        Multiplier,
        Fixed,
    }
    
    [Serializable]
    public struct Stat
    {
        public static float CalculateStats(params Stat[] stats) => CalculateStats(0f, stats);
        public static FlatDamage CalculateStats(FlatDamage baseDamage, params Stat[] stats)
        {
            baseDamage.Damage = CalculateStats(baseDamage.Damage, stats);
            return baseDamage;
        }

        public static int CalculateStats(int baseStat, params Stat[] stats)
        {
            return Mathf.FloorToInt(CalculateStats((float)baseStat, stats));
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
        
        public static float CalculateFlat(Stat[] stats)
        {
            var result = new Stat();

            foreach (var stat in stats)
            {
                result += stat;
            }

            return result._flat;
        }
        
        public static float CalculateMultiplier(Stat[] stats)
        {
            var result = new Stat();

            foreach (var stat in stats)
            {
                result += stat;
            }

            return 1 + result._increase * result._multiplier;
        }
        
        public static Stat[] Transform(StatType finalType, params Stat[] scalers)
        {
            for (var i = 0; i < scalers.Length; i++)
            {
                scalers[i]._statType = finalType;
            }

            return scalers;
        }
        
        public static Stat[] IsolateFactor(StatFactor factor, params Stat[] scalers)
        {
            for (var i = 0; i < scalers.Length; i++)
            {
                if (factor != StatFactor.Flat)
                {
                    scalers[i]._flat = 0;
                }
                if (factor != StatFactor.Increased)
                {
                    scalers[i]._increase = 0;
                }
                if (factor != StatFactor.Multiplier)
                {
                    scalers[i]._multiplier = 0;
                }
                if (factor != StatFactor.Fixed)
                {
                    scalers[i]._fixedStat = 0;
                }
            }
            
            return scalers;
        }
        
        public static Stat[] Scale(float multiplier, params Stat[] scalers)
        {
            for (var i = 0; i < scalers.Length; i++)
            {
                scalers[i]._flat *= multiplier;
                scalers[i]._increase *= multiplier;
                scalers[i]._multiplier *= multiplier;
                scalers[i]._fixedStat *= multiplier;
            }

            return scalers;
        }
        
        public static Stat[] Scale(float multiplier, StatFactor factor, params Stat[] scalers)
        {
            for (var i = 0; i < scalers.Length; i++)
            {
                switch (factor)
                {
                    case StatFactor.Flat:
                        scalers[i]._flat *= multiplier;
                        break;
                    case StatFactor.Increased:
                        scalers[i]._increase *= multiplier;
                        break;
                    case StatFactor.Multiplier:
                        scalers[i]._multiplier *= multiplier;
                        break;
                    case StatFactor.Fixed:
                        scalers[i]._fixedStat *= multiplier;
                        break;
                }
            }

            return scalers;
        }
        
        [SerializeField] private StatType _statType;

        [SerializeField] private float _flat;
        [Percentage] [SerializeField] private float _increase;
        [Percentage] [SerializeField] private float _multiplier;
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

        public string GetDescription(SkillTags conditional)
        {
            if (_fixed)
            {
                return LocalizationManager.LocalizeStat(_statType, StatFactor.Fixed, _fixedStat,
                    conditional);
            }
            
            var returnString = new string[3];
            
            if (_flat != 0)
            {
                returnString[0] = LocalizationManager.LocalizeStat(_statType, StatFactor.Flat, _flat,
                    conditional);
            }
            if (_increase != 0)
            {
                returnString[1] = LocalizationManager.LocalizeStat(_statType, StatFactor.Increased, _increase * 100, 
                    conditional);
            }

            if (_multiplier != 0)
            {
                returnString[2] = LocalizationManager.LocalizeStat(_statType, StatFactor.Multiplier, _multiplier * 100,
                    conditional);
            }

            for (var index = 0; index < returnString.Length; index++)
            {
                if (!string.IsNullOrEmpty(returnString[index]))
                {
                    returnString[index] += "\n";
                }
            }

            return string.Concat(returnString).TrimEnd('\n');
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
