using System;
using System.Collections.Generic;
using Global;
using UnityEditor;
using UnityEngine;

namespace Editor.Inspectors
{
    [CustomEditor(typeof(AllRegistries))]
    public class AllRegistriesInspector : UnityEditor.Editor
    {
        private enum Tab
        {
            Skills,
            Collectables,
            TreeModifiers,
        }

        private Tab _currentTab;
        
        private const string SkillRegistry = "Skills";
        private const string TreeModifierRegistry = "TreeModifiers";
        private const string Entry = "_entry";

        private Vector2 _scroll;
        private string _searchField;

        private SerializedProperty _skills;
        private SerializedProperty _treeModifiers;

        public override void OnInspectorGUI()
        {
            var newTab = (Tab)GUILayout.Toolbar((int)_currentTab, Enum.GetNames(typeof(Tab)));
            if (newTab != _currentTab)
            {
                _scroll = Vector2.zero;
                _searchField = string.Empty;
                _currentTab = newTab;
            }

            switch (_currentTab)
            {
                case Tab.Skills:
                    DrawRegistry(serializedObject.FindProperty(SkillRegistry), "Skill Registry");
                    break;
                case Tab.Collectables:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Collectables"));
                    break;
                case Tab.TreeModifiers:
                    DrawRegistry(serializedObject.FindProperty(TreeModifierRegistry), "Tree Modifier Registry");
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRegistry(SerializedProperty registry, string registryName)
        {
            EditorGUILayout.LabelField(registryName, new GUIStyle("ToolbarBoldLabel"));
            _searchField = EditorGUILayout.TextField(_searchField, new GUIStyle("ToolbarSearchTextField"));
            _scroll = EditorGUILayout.BeginScrollView(_scroll, new GUIStyle("FrameBox"),
                GUILayout.MinHeight(100), GUILayout.MaxHeight(300), GUILayout.ExpandHeight(true));
            {
                foreach (var property in registry.GetDirectChildren())
                {
                    DrawEntry(property, property.displayName);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawEntry(SerializedProperty property, string entryName)
        {
            var entry = property.FindPropertyRelative(Entry);
            var fields = entry.GetDirectChildren();
            if (string.IsNullOrEmpty(_searchField) || entry.type.ToLower().Contains(_searchField.ToLower()))
            {
                EditorGUILayout.BeginVertical(new GUIStyle("IN ThumbnailShadow")
                {
                    padding = new RectOffset(5, 10, 5, 10),
                });
                {
                    EditorGUILayout.LabelField(entryName, new GUIStyle("ToolbarBoldLabel"));
                    EditorGUI.indentLevel = 1;
                    foreach (var field in fields)
                    {
                        EditorGUILayout.PropertyField(field);
                    }
                    EditorGUI.indentLevel = 0;
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
