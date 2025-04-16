using System;
using System.Collections.Generic;
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
            foreach (var connection in node.Connections)
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
        [SerializeField] private NodeParameters[] _nodeParameters;

        public void SelectSkill(SkillNode node)
        {
            
        }

        public void AddNodeToTransform(RectTransform node)
        {
            node.SetParent(_scrollView.content);
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
    }
}
