using System;
using System.Collections.Generic;
using Gameplay.Modifiers;
using UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Elements.Skill_Tree
{
    public enum SkillType
    {
        Root,
        Travel,
        Small,
        Big,
        Power,
    }
    public class SkillNode : MonoBehaviour, IPointerClickHandler
    {
        private Modifier _modifier;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Cluster _cluster;
        [SerializeField] private SkillType _nodeType;
        [SerializeField] private SkillTree _tree;
        [SerializeField] private List<SkillNode> _adjacentNodes;
        [SerializeField] private Image _nodeImage;
        public SkillNode[] Connections => _adjacentNodes.ToArray();
        public SkillType NodeType => _nodeType;

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

        public void SetSkillType(SkillType type)
        {
            _nodeType = type;
            var nodeParams = _tree.GetNode(type);
            if (nodeParams.HasValue)
            {
                var value = nodeParams.Value;
                _rectTransform.sizeDelta = Vector2.one * value.NodeSize;
                _nodeImage.sprite = value.NodeSprite;
            }
        }

        public bool AddCluster(Cluster cluster)
        {
            if (cluster == null)
            {
                return false;
            }

            _cluster = cluster;
            return true;
        }

        public void RemoveConnections()
        {
            foreach (var node in _adjacentNodes)
            {
                node.Detach(this);
            }
            _adjacentNodes.Clear();
        }

        private void OnValidate()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (!_nodeImage && TryGetComponent<Image>(out var image))
            {
                _nodeImage = image;
            }
            else if (!_nodeImage)
            {
                _nodeImage = gameObject.AddComponent<Image>();
            }
        }
    }
}
