using System;
using Gameplay.Modifiers;
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
            Clusters,
        }

        private Tab _currentTab;
        
        private const string SkillRegistry = "Skills";
        private const string TreeModifierRegistry = "TreeModifiers";
        private const string ClusterRegistry = "Clusters";
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
                    DrawRegistry(serializedObject.FindProperty(SkillRegistry), "Skills");
                    break;
                case Tab.Collectables:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Collectables"));
                    break;
                case Tab.TreeModifiers:
                    DrawRegistry(serializedObject.FindProperty(TreeModifierRegistry), "Tree Modifiers");
                    break;
                case Tab.Clusters:
                    DrawClusterRegistry();
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
            var entry = property.FindPropertyRelative(Entry);
            var fields = entry.GetDirectChildren();
            if (string.IsNullOrEmpty(_searchField) || entry.type.ToLower().Contains(_searchField.ToLower()))
            {
                EditorGUILayout.BeginVertical(new GUIStyle("IN ThumbnailShadow")
                {
                    padding = new RectOffset(5, 10, 5, 10),
                });
                {
                    EditorGUILayout.LabelField(property.displayName, new GUIStyle("ToolbarBoldLabel"));
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
        
        private void DrawClusterRegistry()
        {
            var registry = serializedObject.FindProperty(ClusterRegistry);
            var treeMods = serializedObject.FindProperty(TreeModifierRegistry);
            EditorGUILayout.LabelField("Clusters", new GUIStyle("ToolbarBoldLabel"));
            _searchField = EditorGUILayout.TextField(_searchField, new GUIStyle("ToolbarSearchTextField"));
            _scroll = EditorGUILayout.BeginScrollView(_scroll, new GUIStyle("FrameBox"),
                GUILayout.MinHeight(100), GUILayout.MaxHeight(300), GUILayout.ExpandHeight(true));
            {
                foreach (var property in registry.GetDirectChildren())
                {
                    DrawClusterEntry(property, treeMods);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawClusterEntry(SerializedProperty property, SerializedProperty treeMods)
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
                    EditorGUILayout.LabelField(property.displayName, new GUIStyle("ToolbarBoldLabel"));
                    EditorGUI.indentLevel = 1;
                    foreach (var field in fields)
                    {
                        EditorGUILayout.PropertyField(field);
                        for (var i = 0; i < field.arraySize; i++)
                        {
                            var treeModName = field.GetArrayElementAtIndex(i).stringValue;
                            if (string.IsNullOrEmpty(treeModName))
                            {
                                continue;
                            }
                            var treeModEntry = treeMods.FindPropertyRelative(treeModName).FindPropertyRelative(Entry);
                            var treeModObj = treeModEntry.boxedValue as TreeModifier;
                            GUI.enabled = false;
                            EditorGUILayout.TextArea(treeModObj.GetEditorDescription());
                            GUI.enabled = true;
                        }
                    }
                    EditorGUI.indentLevel = 0;
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
