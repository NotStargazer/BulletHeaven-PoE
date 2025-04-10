using UnityEditor;
using UnityEngine;

namespace Editor.Inspectors.SkillTree
{
    [CustomEditor(typeof(UI.Components.SkillTree))]
    public class SkillTreeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                OpenEditor();
            }
        }

        private void DrawNode()
        {
            
        }

        private void OpenEditor()
        {
            SkillTreeEditorWindow.SetTarget((UI.Components.SkillTree)target);
            EditorWindow.CreateWindow<SkillTreeEditorWindow>("Skill Tree Editor");
        }
    }
}
