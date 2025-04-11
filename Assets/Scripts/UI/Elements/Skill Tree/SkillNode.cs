using System.Collections.Generic;
using Gameplay.Modifiers;
using UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Elements.Skill_Tree
{
    public class SkillNode : MonoBehaviour, IPointerClickHandler
    {
        private Modifier[] _modifiers;
        [SerializeField] private SkillTree _tree;
        [SerializeField] private List<SkillNode> _adjacentNodes;
        public SkillNode[] Connections => _adjacentNodes.ToArray();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                _tree.SelectSkill(this);
            }
        }

        public void Attach(SkillNode node)
        {
            if (!_adjacentNodes.Contains(node))
            {
                _adjacentNodes.Add(node);
            }
        }

        public bool HasConnection(SkillNode node)
        {
            return _adjacentNodes.Contains(node);
        }
    }
}
