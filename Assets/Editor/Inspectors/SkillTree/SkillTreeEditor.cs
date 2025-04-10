using UI.Elements.Skill_Tree;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Editor.Inspectors.SkillTree
{
    [EditorTool("Skill Tree Editor", typeof(SkillNode))]
    public class SkillTreeEditor : EditorTool, IDrawSelectedHandles
    {
        public void OnDrawHandles()
        {
            var middle = Vector3.zero;
            var max = Vector2.negativeInfinity;
            var min = Vector2.positiveInfinity;
            var count = 0;
            foreach (var t in targets)
            {
                var node = (SkillNode)t;
                var rt = node.GetComponent<RectTransform>();
                middle += rt.position;
                max = new Vector2(Mathf.Max(max.x, rt.offsetMax.x), Mathf.Max(max.y, rt.offsetMax.y));
                min = new Vector2(Mathf.Min(min.x, rt.offsetMin.x), Mathf.Min(min.y, rt.offsetMin.y));
                count++;
            }
            middle /= count;
            var size = new Vector2(Mathf.Abs(max.x - min.x), Mathf.Abs(max.y - min.y));

            Handles.DrawWireCube(middle, size);
            Handles.FreeMoveHandle(middle, 2, Vector3.one * 0.25f, Handles.RectangleHandleCap);
            Handles.Label(middle, EditorGUIUtility.IconContent("MoveTool"));
        }
    }
}
