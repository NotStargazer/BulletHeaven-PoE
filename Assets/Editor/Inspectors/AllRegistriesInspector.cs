using System;
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
            TreeModifiers
        }

        private static Tab _currentTab;
        
        private const string SKILL_REGISTRY = "Skills";
        private const string TREE_MODIFIER_REGISTRY = "SkillTreeModifiers";
        private const string ENTRY = "_entry";

        private static Vector2 _scroll;
        private static string _searchField;

        private SerializedProperty _skills;
        private SerializedProperty _treeModifiers;

        private static GUIStyle _verticalLayout;

        private static GUIStyle _labelField;
        
        public override void OnInspectorGUI()
        {
            _verticalLayout ??= new GUIStyle("IN ThumbnailShadow")
                { padding = new RectOffset(5, 10, 5, 10) };
            _labelField ??= new GUIStyle("ToolbarBoldLabel");
            
            serializedObject.Update();
            
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
                    DrawRegistry(serializedObject.FindProperty(SKILL_REGISTRY), "Skills");
                    break;
                case Tab.Collectables:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Collectables"));
                    break;
                case Tab.TreeModifiers:
                    DrawRegistry(serializedObject.FindProperty(TREE_MODIFIER_REGISTRY), "Tree Modifiers");
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
                    DrawEntry(property);
                }
            }
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawEntry(SerializedProperty property)
        {
            var entry = property.FindPropertyRelative(ENTRY);
            var fields = entry.GetDirectChildren();
            if (string.IsNullOrEmpty(_searchField) || property.displayName.ToLower().Contains(_searchField.ToLower()))
            {
                EditorGUILayout.BeginVertical(_verticalLayout);
                {
                    EditorGUILayout.LabelField(property.displayName, _labelField);
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
