using UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Elements.Skill_Tree
{
    public class SkillNode : MonoBehaviour, IPointerClickHandler
    {
        private SkillTree _tree;
        private SkillNode[] _childNodes;
        private SkillNode _parent;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                _tree.SelectSkill(this);
            }
        }
    }
}
