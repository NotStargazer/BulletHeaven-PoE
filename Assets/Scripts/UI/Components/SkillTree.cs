using System;
using System.Collections.Generic;
using Gameplay.Modifiers;
using UI.Elements.Skill_Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Components
{
    [Serializable]
    public record Cluster
    {
        [SerializeField] private List<SkillNode> _big;
        [SerializeField] private List<SkillNode> _small;

        public Cluster(SkillNode bigNode)
        {
            _big = new List<SkillNode>();
            _small = new List<SkillNode>();

            if (bigNode.AddCluster(this))
            {
                _big.Add(bigNode);
                SearchAdjacent(bigNode);
            }
        }

        private void SearchAdjacent(SkillNode node)
        {
            foreach (var connection in node.AdjacentNodes)
            {
                if (connection.NodeType == SkillType.Big)
                {
                    if (connection.AddCluster(this))
                    {
                        _big.Add(connection);
                        SearchAdjacent(connection);
                    }
                }
                else if (connection.NodeType == SkillType.Small)
                {
                    if (connection.AddCluster(this))
                    {
                        _small.Add(connection);
                        SearchAdjacent(connection);
                    }
                }
            }
        }

        public int Big => _big.Count;
        public int Small => _small.Count;

        public override int GetHashCode()
        {
            return unchecked(Big * 31 + Small);
        }
    }

    public class SkillTree : MonoBehaviour
    {
        [Serializable]
        public struct NodeParameters
        {
            public SkillType Type;
            public Sprite NodeSprite;
            public float NodeSize;
        }

        [SerializeField] private List<SkillNode> _rootNodes;
        [SerializeField] private List<Cluster> _clusters;
        [SerializeField] private ScrollRect _scrollView;
        [SerializeField] private RectTransform _connectionTransform;
        [SerializeField] private SkillNode _skillNodePrefab;
        [SerializeField] private NodeConnection _connectionPrefab;
        [SerializeField] private NodeParameters[] _nodeParameters;

        private List<SkillNode> _selectedNodes;
        private List<Modifier> _modifiers;
        private float _zoom = 1;

        private void Awake()
        {
            _selectedNodes = new List<SkillNode>();
            _modifiers = new List<Modifier>();
        }

        public void SelectSkill(SkillNode node)
        {
            if (_selectedNodes.Contains(node))
            {
                _selectedNodes.Remove(node);
            }
            else
            {
                _selectedNodes.Add(node);
            }
        }

        public void AllocateNodes()
        {
            foreach (var node in _selectedNodes)
            {
                _modifiers.Add(node.Allocate());
            }
            _selectedNodes.Clear();
        }

        public SkillNode CreateNode(SkillType type)
        {
            var node = Instantiate(_skillNodePrefab, _scrollView.content);
            node.Tree = this;
            node.SetSkillType(type);
            return node;
        }

        public NodeParameters? GetNode(SkillType skillType)
        {
            foreach (var nodeParameter in _nodeParameters)
            {
                if (nodeParameter.Type == skillType)
                {
                    return nodeParameter;
                }
            }

            return null;
        }

        private void OnValidate()
        {
            var size = Enum.GetValues(typeof(SkillType)).Length;
            var values = Enum.GetValues(typeof(SkillType));
            if (_nodeParameters.Length < size)
            {
                Array.Resize(ref _nodeParameters, size);
            }

            for (var i = 0; i < _nodeParameters.Length; i++)
            {
                _nodeParameters[i].Type = (SkillType)values.GetValue(i);
            }
        }

        public NodeConnection CreateConnection(SkillNode a, SkillNode b)
        {
            var connection = Instantiate(_connectionPrefab, _connectionTransform);
            connection.Connect(a, b);
            return connection;
        }

        public void OnScroll(PointerEventData eventData)
        {
            _zoom += eventData.scrollDelta.y;
            _zoom = Mathf.Clamp(_zoom, 1, 3);
            _scrollView.viewport.localScale = Vector3.one * _zoom;
        }
    }
}
