using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UI.Localization;
using UnityEditor;
using UnityEngine;
using LocalizationAsset = UI.Localization.LocalizationAsset;

namespace Editor.Inspectors
{
    [CustomEditor(typeof(LocalizationAsset))]
    public class LocalizationAssetInspector : UnityEditor.Editor
    {
        private record LocHierarchy
        {
            private Dictionary<string, LocHierarchy> _children;
            private SerializedProperty _property;
            private string _label;
            

            public LocHierarchy()
            {
                _children = new Dictionary<string, LocHierarchy>();
            }

            public void Add(Queue<string> remainingKeys, SerializedProperty property, string label = "")
            {
                _label = label;
                if (remainingKeys.Count == 0)
                {
                    _property = property;
                    return;
                }

                var key = remainingKeys.Dequeue();
                if (_children.ContainsKey(key))
                {
                    _children[key].Add(remainingKeys, property, key);
                }
                else
                {
                    var lh = new LocHierarchy();
                    _children.Add(key, lh);
                    lh.Add(remainingKeys, property, key);
                }
            }

            public void Draw(Dictionary<string, FormattedValue> values, int depth = 0)
            {
                EditorGUI.indentLevel = depth - 1;
                if (depth == 1)
                {
                    EditorGUILayout.BeginVertical(new GUIStyle("IN Footer"));
                }

                if (!string.IsNullOrEmpty(_label))
                {
                    EditorGUILayout.LabelField(_label, new GUIStyle("CN Box"));
                }

                if (_property != null)
                {
                    var controlRect = EditorGUILayout.GetControlRect(false);
                    GUI.contentColor = Color.clear;
                    var newValue = EditorGUI.TextArea(controlRect, _property.stringValue);
                    GUI.contentColor = Color.white;
                    if (_property.stringValue != newValue || !values.ContainsKey(_property.propertyPath))
                    {
                        var value = new FormattedValue();
                        _property.stringValue = newValue;
                        var result = Regex.Replace(_property.stringValue, "#\\d#", match =>
                        {
                            value.Variables++;
                            return $"<color=#55ff55>{match.Value}</color>";
                        });
                        result = Regex.Replace(result, "\\{\\w*:.+\\}", match =>
                        {
                            value.Formatters++;
                            return $"<color=#5555ff>{match.Value}</color>";
                        });
                        value.Text = result;
                        if (!values.TryAdd(_property.propertyPath, value))
                        {
                            values[_property.propertyPath] = value;
                        }
                    }

                    if (values.TryGetValue(_property.propertyPath, out var formattedValue))
                    {
                        EditorGUI.LabelField(controlRect, formattedValue.Text, new GUIStyle("CN Message"));
                        EditorGUILayout.LabelField($"<color=#55ff55>{formattedValue.Variables}</color> Variables | <color=#5555ff>{formattedValue.Formatters}</color> Formaters", new GUIStyle("CN Message"));
                    }
                }

                foreach (var child in _children.Values)
                {
                    child.Draw(values, depth + 1);
                }

                if (depth == 1)
                {
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private struct FormattedValue
        {
            public string Text;
            public int Variables;
            public int Formatters;
        }

        private LocalizationManager.LocalizationKey _localizationKey;
        private string _searchField;
        private LocHierarchy _rootHierarchy;
        private Dictionary<string, FormattedValue> _formattedValues;

        private void Awake()
        {
            _formattedValues = new Dictionary<string, FormattedValue>();
        }

        public override void OnInspectorGUI()
        {
            _rootHierarchy = new LocHierarchy();
            _localizationKey = (LocalizationManager.LocalizationKey)EditorGUILayout.EnumPopup("Current Localization", _localizationKey);
            var entries = serializedObject.FindProperty("_entries");
            _searchField = EditorGUILayout.TextField(_searchField, new GUIStyle("ToolbarSearchTextField"));

            for (var i = 0; i < entries.arraySize; i++)
            {
                var entry = entries.GetArrayElementAtIndex(i);
                var key = entry.FindPropertyRelative("Key");
                var currentString = entry.FindPropertyRelative("Strings").GetArrayElementAtIndex((int)_localizationKey);
                if (string.IsNullOrEmpty(_searchField) || key.stringValue.ToLower().Contains(_searchField.ToLower()))
                {
                    _rootHierarchy.Add(new Queue<string>(key.stringValue.Split('.')), currentString);
                }
            }

            _rootHierarchy.Draw(_formattedValues);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
