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

                if (_property != null && values != null)
                {
                    EditorGUI.indentLevel = 0;
                    var controlRect = EditorGUILayout.GetControlRect(false, -2);
                    GUI.contentColor = Color.clear;
                    var newValue = EditorGUILayout.TextArea(_property.stringValue, new GUIStyle(EditorStyles.textArea)
                    {
                        clipping = TextClipping.Overflow,
                        margin = new RectOffset(3 + 15 * (depth - 1), 0, 0, 0)
                    });
                    GUI.contentColor = Color.white;
                    EditorGUI.indentLevel = depth - 1;
                    
                    if (_property.stringValue != newValue || !values.ContainsKey(_property.propertyPath))
                    {
                        var value = new FormattedValue();
                        _property.stringValue = newValue;
                        
                        var result = Regex.Replace(_property.stringValue, "#\\d#", match =>
                        {
                            value.Variables++;
                            return $"<color=#55ff55>{match.Value}</color>";
                        });
                        result = Regex.Replace(result, "\\{.*?\\|.*?\\}", match =>
                        {
                            value.Formatters++;
                            return $"<color=#7777ff>{match.Value}</color>";
                        });
                        value.Text = result;
                        if (!values.TryAdd(_property.propertyPath, value))
                        {
                            values[_property.propertyPath] = value;
                        }
                    }
                    
                    if (values.TryGetValue(_property.propertyPath, out var formattedValue))
                    {
                        var error = 0;
                        for (var i = 1; i < formattedValue.Variables + 1; i++)
                        {
                            if (!_property.stringValue.Contains($"#{i}#"))
                            {
                                error = i;
                                break;
                            }
                        }

                        if (error != 0)
                        {
                            formattedValue.Text = formattedValue.Text.Replace("#55ff55", "#ff0000");
                        }

                        controlRect.width -= 2;
                        EditorGUI.LabelField(controlRect, formattedValue.Text, new GUIStyle("CN Message")
                        {
                            clipping = TextClipping.Overflow
                        });
                        EditorGUILayout.LabelField($"<color=#55ff55>{formattedValue.Variables}</color> Variables | <color=#7777ff>{formattedValue.Formatters}</color> Formaters", new GUIStyle("CN Message"));
                        if (error != 0)
                        {
                            EditorGUILayout.LabelField($"<color=#ff0000>Variables must be in ascending order starting from 1\nmissing #{error}#", new GUIStyle("CN Message"));
                        }
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

        private Dictionary<string, FormattedValue> _formattedValues;
        private static LocalizationManager.LocalizationKey LOCALIZATION_KEY;
        private static string SEARCH_FIELD;

        private void Awake()
        {
            _formattedValues ??= new Dictionary<string, FormattedValue>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var rootHierarchy = new LocHierarchy();
            LOCALIZATION_KEY = (LocalizationManager.LocalizationKey)EditorGUILayout.EnumPopup("Current Localization", LOCALIZATION_KEY);
            var entries = serializedObject.FindProperty("_entries");
            SEARCH_FIELD = EditorGUILayout.TextField(SEARCH_FIELD, new GUIStyle("ToolbarSearchTextField"));

            for (var i = 0; i < entries.arraySize; i++)
            {
                var entry = entries.GetArrayElementAtIndex(i);
                var key = entry.FindPropertyRelative("Key");
                var currentString = entry.FindPropertyRelative("Strings").GetArrayElementAtIndex((int)LOCALIZATION_KEY);
                if (string.IsNullOrEmpty(SEARCH_FIELD) || key.stringValue.ToLower().Contains(SEARCH_FIELD.ToLower()))
                {
                    rootHierarchy.Add(new Queue<string>(key.stringValue.Split('.')), currentString);
                }
            }

            rootHierarchy.Draw(_formattedValues);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
