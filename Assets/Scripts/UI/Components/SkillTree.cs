using System;
using System.Collections.Generic;
using Gameplay.Modifiers;
using TMPro;
using UI.Elements.Skill_Tree;
using UnityEngine;
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

    [SelectionBase]
    public class SkillTree : BaseMenu, IModifierSource
    {
        [Serializable]
        public struct NodeParameters
        {
            public SkillType Type;
            public Material NodeMaterial;
            public float NodeSize;
        }

        [SerializeField] private TMP_Text _availablePoints;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _openButton;
        [SerializeField] private List<SkillNode> _rootNodes;
        [SerializeField] private List<Cluster> _clusters;
        [SerializeField] private ZoomableScrollRect _scrollView;
        [SerializeField] private RectTransform _connectionTransform;
        [SerializeField] private SkillNode _skillNodePrefab;
        [SerializeField] private NodeConnection _connectionPrefab;
        [SerializeField] private NodeParameters[] _nodeParameters;

        private List<SkillNode> _selectedNodes;
        private List<Modifier> _modifiers;
        private int _totalSkillPoints;

        public int Points => _totalSkillPoints - (_modifiers.Count + _selectedNodes.Count);
        
        protected override void MenuAwake()
        {
            _selectedNodes = new List<SkillNode>();
            _modifiers = new List<Modifier>();
            _applyButton.onClick.AddListener(AllocateNodes);
            _cancelButton.onClick.AddListener(Cancel);
            _closeButton.onClick.AddListener(() =>
            {
                Cancel();
                SetVisible(false);
            });
            _openButton.onClick.AddListener(() =>
            {
                SetVisible(true);
            });
            _applyButton.interactable = false;
            _cancelButton.interactable = false;

            foreach (var skillNode in _rootNodes)
            {
                skillNode.SetState(NodeState.Unlocked);
            }
        }

        public bool SelectSkill(SkillNode node)
        {
            if (_selectedNodes.Contains(node))
            {
                _selectedNodes.Remove(node);
                UpdateAvailablePoints();
                return true;
            }

            if (_modifiers.Count + _selectedNodes.Count < _totalSkillPoints)
            {
                _selectedNodes.Add(node);
                UpdateAvailablePoints();
                return true;
            }

            return false;
        }

        private void UpdateAvailablePoints()
        {
            var hasSelected = _selectedNodes.Count > 0;
            _applyButton.interactable = hasSelected;
            _cancelButton.interactable = hasSelected;
            _openButton.gameObject.SetActive(Points > 0);
            _availablePoints.SetText(Points.ToString());
        }

        private void AllocateNodes()
        {
            foreach (var node in _selectedNodes)
            {
                _modifiers.Add(node.Allocate());
            }

            _selectedNodes.Clear();
            UpdateAvailablePoints();
        }

        private void Cancel()
        {
            for (var index = _selectedNodes.Count - 1; index >= 0; index--)
            {
                var node = _selectedNodes[index];
                node.SetState(NodeState.Unlocked);
            }

            _selectedNodes.Clear();
            UpdateAvailablePoints();
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

        public void AddPoint()
        {
            _totalSkillPoints++;
            UpdateAvailablePoints(); 
        }

        public IEnumerable<Modifier> GetModifiers()
        {
            return _modifiers;
        }
    }
}
