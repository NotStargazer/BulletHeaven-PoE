using System.Collections.Generic;
using Gameplay.Modifiers;
using UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Elements.Skill_Tree
{
    public enum SkillType
    {
        Travel,
        Small,
        Big,
        Power
    }
    
    public class SkillNode : MonoBehaviour, IPointerClickHandler
    {
        private Modifier[] _modifiers;
        [SerializeField] private SkillTree _tree;
        [SerializeField] private List<SkillNode> _adjacentNodes;
        [SerializeField] private RawImage _node;
        public SkillNode[] Connections => _adjacentNodes.ToArray();

        public SkillTree Tree
        {
            get => _tree;
            set => _tree = value;
        }

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

        public void Detach(SkillNode node)
        {
            _adjacentNodes.Remove(node);
        }

        public bool HasConnection(SkillNode node)
        {
            return _adjacentNodes.Contains(node);
        }

        public void SetSkillType()
        {
            
        }
    }
}
