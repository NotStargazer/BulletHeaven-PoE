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
        private readonly string _key;
        private string _final;
        private int _variable;
        
        public LocalizationBuilder(string key, string s)
        {
            _key = key;
            _final = s;
            _variable = 1;
        }

        public LocalizationBuilder SelectFormatter(bool selectOther)
        {
            var first = _final.IndexOf('{');
            var split = _final.IndexOf('|');
            var last = _final.IndexOf('}');
            var formatter = _final.Substring(first, last - first + 1);

            if (first == -1 || split == -1 || last == -1)
            {
                throw new LocalizationException($"{_key} has invalid formatter syntax.");
            }

            _final = _final.Replace(formatter, selectOther 
                ? _final.Substring(split + 1, last) 
                : _final.Substring(first + 1, split));

            return this;
        }
        
        public LocalizationBuilder ReplaceVariable(object variable)
        {
            var v = $"#{_variable}#";
            _variable++;
            
            if (_final.Contains(v))
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
        public static string LocalizeStat(StatType statType, StatFactor statFactor, float value, string append = "")
        {
            var key = $"{statType.BaseName}.{statFactor}.{statType.SubName}";
#if UNITY_EDITOR
            var loc = GetEditorLocalization("Stats", key);
#else
            var loc = Localization.GetLocalization(key);
#endif

            var negative = Mathf.Sign(value) < 0;
            var hasCondition = !string.IsNullOrEmpty(append);
            
            return loc
                .SelectFormatter(negative)
                .ReplaceVariable(value)
                .ReplaceVariable(append)
                .SelectFormatter(hasCondition)
                .Build();
        }

        public static string GetCondition(SkillTags condition)
        {
            var key = $"Condition.{condition.ToString()}";
            
#if UNITY_EDITOR
            var loc = GetEditorLocalization("Stats", key);
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
