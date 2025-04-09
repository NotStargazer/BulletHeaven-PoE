using System;
using UnityEngine;

namespace Gameplay.Modifiers
{
    public class StatName
    {
        public static class Damage
        {
            private static string GetName(string name) => $"Damage.{name}";

            public static string Physical => GetName("Physical");
            public static string Fire => GetName("Fire");
            public static string Cold => GetName("Cold");
            public static string Lightning => GetName("Lightning");
        }
    }
    
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
        
        [SerializeField] private string _statName;

        [SerializeField] private float _flat;
        [SerializeField] private float _increase;
        [SerializeField] private float _multiplier;
        [SerializeField] private bool _fixed;
        [SerializeField] private float _fixedStat;

        public Stat(string name, float flat = 0, float increase = 0, float multiplier = 1)
        {
            _statName = name;
            
            _flat = flat;
            _increase = increase;
            _multiplier = multiplier;
            _fixed = false;
            _fixedStat = Mathf.Infinity;
        }

        public Stat(string name, float fixedStat)
        {
            _statName = name;
            
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
                return new Stat(lhs._statName, Mathf.Min(lhs._flat, rhs._flat));
            }

            return new Stat(lhs._statName,
                lhs._flat + rhs._flat,
                lhs._increase + rhs._increase,
                lhs._multiplier * rhs._multiplier);
        }
    }
}
