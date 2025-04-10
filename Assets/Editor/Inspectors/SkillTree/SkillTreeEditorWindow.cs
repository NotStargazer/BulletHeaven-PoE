using UI.Elements.Skill_Tree;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Editor.Inspectors.SkillTree
{
    public class SkillTreeEditorWindow : EditorWindow, IDrawSelectedHandles
    {
        private static UI.Components.SkillTree TARGET;
        private static SkillNode[] NODES;
        private static UnityEditor.Editor EDITOR;

        private void OnSelectionChange()
        {
            NODES = Selection.GetFiltered<SkillNode>(SelectionMode.Unfiltered);
        }

        private void OnDestroy()
        {
            TARGET = null;
            NODES = null;
            EDITOR.Cleanup();
            EDITOR = null;
        }

        public static void SetTarget(UI.Components.SkillTree target)
        {
            TARGET = target;
        }

        public void OnDrawHandles()
        {

        }
    }
}
