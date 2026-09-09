using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utility;

namespace UI.Localization
{
    public class Localization : ProtectedSingletonBehaviour<Localization>
    {
        [SerializeField] private LocalizationManager.LocalizationKey _currentLang;
        [SerializeField] private LocalizationAsset[] _localizationAssets;
        private Dictionary<string, string[]> _stringList;
        
        public override void Instantiate()
        {
            _stringList = new Dictionary<string, string[]>();
            foreach (var localizationAsset in _localizationAssets)
            {
                localizationAsset.AppendStrings(_stringList);
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            var assets = AssetDatabase.FindAssets("t:LocalizationAsset", new[] { "Assets/Localization" });
            _localizationAssets = new LocalizationAsset[assets.Length];
            for (var index = 0; index < assets.Length; index++)
            {
                _localizationAssets[index] = AssetDatabase.LoadAssetAtPath<LocalizationAsset>(AssetDatabase.GUIDToAssetPath(assets[index]));
            }
        }

        public static LocalizationBuilder GetLocalization(string key)
        {
            return new LocalizationBuilder(key, Instance._stringList[key][(int)Instance._currentLang]);
        }
#endif
    }
}
