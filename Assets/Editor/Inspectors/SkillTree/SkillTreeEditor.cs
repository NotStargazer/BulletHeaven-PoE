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
            CalculateMiddleAndSize(out var middle, out var size);
            Handles.DrawWireCube(middle, size);
        }
        
        

        public override bool gridSnapEnabled => true;

        private void CalculateMiddleAndSize(out Vector3 middle, out Vector3 size)
        {
            middle = Vector3.zero;
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
            size = new Vector2(Mathf.Abs(max.x - min.x), Mathf.Abs(max.y - min.y));
        }
    }
}
