using UI.Localization;
using UnityEditor;
using UnityEngine;
using LocalizationAsset = UI.Localization.LocalizationAsset;

namespace Editor.Inspectors
{
    [CustomEditor(typeof(LocalizationAsset))]
    public class LocalizationAssetInspector : UnityEditor.Editor
    {
        private LocalizationManager.LocalizationKey _localizationKey;
        private string _searchField;
        public override void OnInspectorGUI()
        {
            _localizationKey = (LocalizationManager.LocalizationKey)EditorGUILayout.EnumPopup("Current Localization", _localizationKey);
            var entries = serializedObject.FindProperty("_entries");
            _searchField = EditorGUILayout.TextField(_searchField, new GUIStyle("ToolbarSearchTextField"));
            
            for (var i = 0; i < entries.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(new GUIStyle("GroupBox"));
                var entry = entries.GetArrayElementAtIndex(i);
                var key = entry.FindPropertyRelative("Key");
                var currentString = entry.FindPropertyRelative("Strings").GetArrayElementAtIndex((int)_localizationKey);
                if (string.IsNullOrEmpty(_searchField) || key.stringValue.ToLower().Contains(_searchField.ToLower()))
                {
                    EditorGUILayout.LabelField(key.stringValue, new GUIStyle("BoldLabel"));
                    currentString.stringValue = EditorGUILayout.TextArea(currentString.stringValue);
                    EditorGUILayout.EndVertical();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
