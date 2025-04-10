using Gameplay.Modifiers;
using UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Elements.Skill_Tree
{
    public class SkillNode : MonoBehaviour, IPointerClickHandler
    {
        private Modifier[] _modifiers;
        private SkillTree _tree;
        private SkillNode[] _adjacentNodes;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                _tree.SelectSkill(this);
            }
        }
    }
}
