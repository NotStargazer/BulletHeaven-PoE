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
            TreeModifiers
        }

        private static Tab _currentTab;
        
        private const string SKILL_REGISTRY = "Skills";
        private const string TREE_MODIFIER_REGISTRY = "TreeModifiers";
        private const string CLUSTER_REGISTRY = "Clusters";
        private const string ENTRY = "_entry";

        private static Vector2 _scroll;
        private static string _searchField;

        private SerializedProperty _skills;
        private SerializedProperty _treeModifiers;

        public override void OnInspectorGUI()
        {
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
            var registry = serializedObject.FindProperty(CLUSTER_REGISTRY);
            var treeMods = serializedObject.FindProperty(TREE_MODIFIER_REGISTRY);
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
            var entry = property.FindPropertyRelative(ENTRY);
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
                            var treeModEntry = treeMods.FindPropertyRelative(treeModName).FindPropertyRelative(ENTRY);
                            var treeModObj = treeModEntry.boxedValue as TreeModifier;
                            GUI.enabled = false;
                            EditorGUILayout.TextArea(treeModObj.GetDescription());
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
