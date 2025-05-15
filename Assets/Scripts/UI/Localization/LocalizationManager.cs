using System;
using System.Collections.Generic;
using System.IO;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using Utility;

namespace UI.Localization
{
    public static class LocalizationManager
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
            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid); //
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
        #endif
    }
}
