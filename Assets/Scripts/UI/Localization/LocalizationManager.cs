using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utility;

namespace UI.Localization
{
    public class LocalizationManager : SingletonBehaviour<LocalizationManager>
    {
        public enum LocalizationKey
        {
            en_US,
            sv_SE,
        }

        public static string[] LocalizationKeys => Enum.GetNames(typeof(LocalizationKey));

#if UNITY_EDITOR
        public static void ValidateLocalizationFile(HashSet<string> keys, string file)
        {
            var assets = AssetDatabase.FindAssets("t:LocalizationAsset", new[] { "Assets/Localization" });
            foreach (var name in assets)
            {
                if (name == file)
                {
                    AssetDatabase.LoadAssetAtPath<LocalizationAsset>(name).Validate(keys);
                    return;
                }
            }

            var so = ScriptableObject.CreateInstance<LocalizationAsset>();
            so.name = file;
            AssetDatabase.CreateAsset(so, $"Assets/Localization/{file}.asset");
            so.Validate(keys);
        }
        #endif


        public override void Instantiate()
        {
            
        }
    }
}
