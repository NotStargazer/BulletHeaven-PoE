using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gameplay;
using Gameplay.Data;
using Gameplay.Modifiers;
using UnityEditor;
using UnityEngine;

namespace UI.Localization
{
    public struct LocalizationBuilder
    {
        public static string FormatterCharacters => string.Concat(Enum.GetNames(typeof(LocalizationFormatter.ValidFormatters)));

        private static class LocalizationFormatter
        {
            public enum ValidFormatters
            {
                R,
                P
            }

            public static string Format(string input, int value, float variable)
            {
                if (Enum.TryParse(input, out ValidFormatters formatter))
                {
                    return formatter switch
                    {
                        ValidFormatters.R => variable.ToString($"F{value}"),
                        ValidFormatters.P => variable.ToString("F0").PadLeft(value, '0'),
                        _ => string.Empty
                    };
                }

                return variable.ToString("F0");
            }
        }
        
        private readonly string _key;
        private string _final;
        private int _variable;
        
        public LocalizationBuilder(string key, string s)
        {
            _key = key;
            _final = s;
            _variable = 1;
        }

        public LocalizationBuilder SetConditional(bool selectOther)
        {
            var first = _final.IndexOf('{');
            var split = _final.IndexOf('|');
            var last = _final.IndexOf('}');
            var conditional = _final.Substring(first, last - first + 1);

            if (first == -1 || split == -1 || last == -1)
            {
                throw new LocalizationException($"{_key} has invalid formatter syntax.");
            }

            var selected = selectOther
                ? _final.Substring(split + 1, last - split - 1)
                : _final.Substring(first + 1, split - first - 1);
            _final = _final.Replace(conditional, selected);

            return this;
        }

        public LocalizationBuilder ReplaceVariable(float variable)
        {
            var v = $"#{_variable}#";
            _variable++;
            
            if (!_final.Contains(v))
            {
                throw new LocalizationException($"Could not find variable {v} in {_key}");
            }

            _final = _final.Replace(v, FormatSingle(variable));

            return this;
        }

        private string FormatSingle(float variable)
        {
            var endIndex = _final.IndexOf('#') - 1;
            if (endIndex > 0 && _final[endIndex - 1] == ']')
            {
                var startIndex = endIndex - 1;
                for (; startIndex > 0; startIndex--)
                {
                    if (_final[startIndex] == '[')
                    {
                        break;
                    }
                }

                var formatter = _final.Remove(startIndex, endIndex);
                var split = formatter.Trim('[', ']').Split('=');
                return LocalizationFormatter.Format(split[0], int.Parse(split[1]), Mathf.Abs(variable));
            }

            return variable.ToString("F0");
        }
        
        public LocalizationBuilder ReplaceVariable(object variable)
        {
            var v = $"#{_variable}#";
            _variable++;
            
            if (!_final.Contains(v))
            {
                throw new LocalizationException($"Could not find variable {v} in {_key}");
            }

            _final = _final.Replace(v, variable.ToString());

            return this;
        }
        
        public string Build()
        {
            return _final;
        }
    }
    
    public static class LocalizationManager
    {
        public enum LocalizationKey
        {
            en_US,
            sv_SE,
        }
        
        
        public static string[] LanguageKeys => Enum.GetNames(typeof(LocalizationKey));

#if UNITY_EDITOR
        public static void ValidateLocalizationFile(HashSet<string> keys, string file)
        {
            var assets = AssetDatabase.FindAssets("t:LocalizationAsset", new[] { "Assets/Localization" });
            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var name = Path.GetFileNameWithoutExtension(path);
                if (name == file)
                {
                    AssetDatabase.LoadAssetAtPath<LocalizationAsset>(path).Validate(keys);
                    AssetDatabase.SaveAssets();
                    return;
                }
            }

            var so = ScriptableObject.CreateInstance<LocalizationAsset>();
            so.name = file;
            AssetDatabase.CreateAsset(so, $"Assets/Localization/{file}.asset");
            so.Validate(keys);
            AssetDatabase.SaveAssetIfDirty(so);
        }

        public static string GetEditorLocalization(string assetName, string[] keys)
        {
            return keys.Aggregate(string.Empty,
                (current, key) => current + $"{GetEditorLocalization(assetName, key)}\n").TrimEnd();
            
        }
        
        public static LocalizationBuilder GetEditorLocalization(string assetName, string key)
        {
            return new LocalizationBuilder(key,
                AssetDatabase.LoadAssetAtPath<LocalizationAsset>($"Assets/Localization/{assetName}.asset").Get(key));
        }
#endif
        public static string LocalizeStat(StatType statType, StatFactor statFactor, float value, string conditional)
        {
            value = Mathf.Abs(value);
            var key = $"{statType.BaseName}.{statFactor}.{statType.SubName}";
#if UNITY_EDITOR
            var loc = GetEditorLocalization("Stats", key);
#else
            var loc = Localization.GetLocalization(key);
#endif

            var negative = Mathf.Sign(value) < 0;
            var hasCondition = !string.IsNullOrEmpty(conditional);
            
            return loc
                .SetConditional(negative)
                .ReplaceVariable(value)
                .ReplaceVariable(conditional)
                .SetConditional(hasCondition)
                .Build();
        }

        public static string GetCondition(SkillTags condition, string type)
        {
            var key = $"Condition.{type}.{condition.ToString()}";
            
#if UNITY_EDITOR
            var loc = GetEditorLocalization("Conditions", key);
#else
            var loc = Localization.GetLocalization(key);
#endif

            return loc.Build();
        }
    }

    public class LocalizationException : Exception
    {
        public LocalizationException(string message) : base(message) { }
    }
}
