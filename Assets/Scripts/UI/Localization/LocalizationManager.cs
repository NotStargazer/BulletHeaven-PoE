using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gameplay;
using Gameplay.Data;
using Gameplay.Modifiers;
using UnityEngine;
using Utility.Attributes;

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
                        ValidFormatters.R => variable.ToString($"F{value}", CultureInfo.InvariantCulture),
                        ValidFormatters.P => variable.ToString("").PadLeft(value, '0'),
                        _ => string.Empty
                    };
                }

                return variable.ToString("F0");
            }
        }
        
        private string _locString;
        public bool IsEmpty => string.IsNullOrEmpty(_locString);
        
        public LocalizationBuilder(string s)
        {
            _locString = s;
        }

        public LocalizationBuilder SetConditional(string identifier, bool selectOther)
        {
            _locString = Regex.Replace(_locString, "\\{\\\"" + identifier + "\\\".*?\\|.*?\\}", match =>
            {
                var val = match.Value.Replace($"\"{identifier}\"", "");
                var split = val.Split('|');
                return selectOther
                    ? split[1].TrimEnd('}')
                    : split[0].TrimStart('{');
            });

            return this;
        }

        public LocalizationBuilder ReplaceVariable(string identifier, float variable)
        {
            var v = $"#{identifier}#";
            var s = FormatSingle(v, variable);
            _locString = _locString.Replace(v, s);
            return this;
        }

        private string FormatSingle(string identifier, float variable)
        {
            var endIndex = _locString.IndexOf(identifier, StringComparison.InvariantCulture) - 1;
            if (endIndex > 0 && _locString[endIndex] == ']')
            {
                var startIndex = endIndex - 1;
                for (; startIndex > 0; startIndex--)
                {
                    if (_locString[startIndex] == '[')
                    {
                        break;
                    }
                }

                var formatter = _locString[(startIndex + 1)..endIndex];
                _locString = _locString.Remove(startIndex, 5);
                var split = formatter.Split('=');
                return LocalizationFormatter.Format(split[0], int.Parse(split[1]), Mathf.Abs(variable));
            }

            return variable.ToString("F0");
        }
        
        public LocalizationBuilder ReplaceVariable(string identifier, object variable)
        {
            var v = $"#{identifier}#";

            _locString = _locString.Replace(v, variable.ToString());

            return this;
        }
        
        public string Build()
        {
            return _locString;
        }
    }
    
    public static class LocalizationManager
    {
        // ReSharper disable InconsistentNaming
        public enum LocalizationKey
        {
            en_US,
            sv_SE,
        }
        // ReSharper restore InconsistentNaming
        
        
        public static string[] LanguageKeys => Enum.GetNames(typeof(LocalizationKey));

#if UNITY_EDITOR
        public static void ValidateLocalizationFile(HashSet<string> keys, string file)
        {
            var assets = UnityEditor.AssetDatabase.FindAssets("t:LocalizationAsset", new[] { "Assets/Localization" });
            foreach (var guid in assets)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var name = Path.GetFileNameWithoutExtension(path);
                if (name == file)
                {
                    UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizationAsset>(path).Validate(keys);
                    UnityEditor.AssetDatabase.SaveAssets();
                    return;
                }
            }

            var so = ScriptableObject.CreateInstance<LocalizationAsset>();
            so.name = file;
            UnityEditor.AssetDatabase.CreateAsset(so, $"Assets/Localization/{file}.asset");
            so.Validate(keys);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(so);
        }

        public static string GetEditorLocalization(string assetName, string[] keys)
        {
            return keys.Aggregate(string.Empty,
                (current, key) => current + $"{GetEditorLocalization(assetName, key)}\n").TrimEnd();
        }
        
        public static LocalizationBuilder GetEditorLocalization(string assetName, string key, string fallbackKey = null)
        {
            var result = UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizationAsset>
                    ($"Assets/Localization/{assetName}.asset").Get(key);
            return new LocalizationBuilder(result);
        }
#endif
        public static string LocalizeStat(StatType statType, StatFactor statFactor, float value, SkillTags conditional)
        {
            var specificSkill = statType.GetConditional(statFactor) == ConditionalType.Flat;
            var condition = conditional == 0
                ? string.Empty
                : GetCondition(conditional, statType.GetConditionalKey(statFactor));
            
            value = Mathf.Abs(value);
            var statTypeKey = $"{statType.BaseName}.{statType.SubName}";
            var statModKey = $"{statType.BaseName}.{statFactor}.{statType.SubName}";
            var statModKeyFallback = $"Generic.{statFactor}";
#if UNITY_EDITOR
            var statMod = GetEditorLocalization("Stat Modifier Overrides", statModKey);
            if (statMod.IsEmpty)
            {
                statMod = GetEditorLocalization("Stat Modifiers", statModKeyFallback);
            }
            var statTypeLoc = GetEditorLocalization("Stat Types", statTypeKey);
#else
            var statMod = Localization.GetLocalization(statModKey, statModKeyFallback);
            var statTypeLoc = Localization.GetLocalization(statTypeKey);
#endif

            var negative = Mathf.Sign(value) < 0;
            var hasCondition = conditional > 0;

            statMod = statMod.SetConditional("Sign", negative)
                .ReplaceVariable("StatType", statTypeLoc.Build())
                .ReplaceVariable("Value", value)
                .SetConditional("Conditional", hasCondition && !specificSkill)
                .SetConditional("Specific", hasCondition && specificSkill)
                .SetConditional("Plural", value > 1);
            if (hasCondition)
            {
                statMod = statMod.ReplaceVariable(specificSkill ? "SkillType" : "Condition", condition);
            }
            return statMod.Build();
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
}
