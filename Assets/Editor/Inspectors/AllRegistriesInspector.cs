using Gameplay;
using Global;
using UnityEditor;
using UnityEngine;

namespace Editor.Inspectors
{
    [CustomEditor(typeof(AllRegistries))]
    public class AllRegistriesInspector : UnityEditor.Editor
    {
        private const string SkillRegistry = "Skills";
        private const string Entry = "_entry";

        private Vector2 _skillScroll;
        private string _searchField;
        
        public override void OnInspectorGUI()
        {
            var skills = serializedObject.FindProperty(SkillRegistry);

            EditorGUILayout.LabelField("Skill Registry", new GUIStyle("ToolbarBoldLabel"));
            _searchField = EditorGUILayout.TextField(_searchField, new GUIStyle("ToolbarSearchTextField"));
            _skillScroll = EditorGUILayout.BeginScrollView(_skillScroll, new GUIStyle("FrameBox"),
                GUILayout.MinHeight(100), GUILayout.MaxHeight(300), GUILayout.ExpandHeight(true));
            {
                var list = skills.GetDirectChildren();
                foreach (var property in list)
                {
                    DrawSkill(property);
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Collectables"));

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSkill(SerializedProperty skill)
        {
            var entry = skill.FindPropertyRelative(Entry);
            var fields = entry.GetDirectChildren();
            if (string.IsNullOrEmpty(_searchField) || entry.type.ToLower().Contains(_searchField.ToLower()))
            {
                EditorGUILayout.BeginVertical(new GUIStyle("IN ThumbnailShadow")
                {
                    padding = new RectOffset(5, 10, 5, 10),
                });
                {
                    EditorGUILayout.LabelField(entry.type, new GUIStyle("ToolbarBoldLabel"));
                    EditorGUI.indentLevel = 1;
                    foreach (var field in fields)
                    {
                        EditorGUILayout.PropertyField(field);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
