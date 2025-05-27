using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Gameplay.Modifiers;
using UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Elements.Skill_Tree
{
    public enum NodeState
    {
        Locked,
        Unlocked,
        Selected,
        Allocated
    }
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
        [SerializedDictionary("Adjacent Node", "Connection")]
        [SerializeField] private SerializedDictionary<SkillNode, NodeConnection> _connections;
        [SerializeField] private Image _nodeImage;
        [SerializeField] private bool _fixed;
        [SerializeField] private StatModifier _fixedModifier;
        public SkillNode[] AdjacentNodes => _connections.Keys.ToArray();
        public NodeConnection[] Connections => _connections.Values.ToArray();
        public SkillType NodeType => _nodeType;

        private NodeState _state;
        public Action<NodeState> OnStateUpdated { get; set; }

        public SkillTree Tree
        {
            get => _tree;
            set => _tree = value;
        }

        public bool Fixed
        {
            get => _fixed;
            set => _fixed = value;
        }

        private void SelectNode()
        {
            if (_state == NodeState.Locked)
            {
                return;
            }
            
            if (_tree.SelectSkill(this))
            {
                SetState(_state == NodeState.Selected ? NodeState.Unlocked : NodeState.Selected);
            }
        }

        public void SetState(NodeState state)
        {
            _state = state;
            OnStateUpdated?.Invoke(_state);

            //Properties passed down to shader
            //R: Brightness, G: Highlight, B: Blink, A: Alpha
            switch (_state)
            {
                case NodeState.Locked:
                    foreach (var node in AdjacentNodes)
                    {
                        node.SetState(NodeState.Locked);
                    }
                    _nodeImage.color = new Color(0.5f, 0f, 0f);
                    break;
                case NodeState.Unlocked:
                    break;
                case NodeState.Selected:
                    foreach (var node in AdjacentNodes)
                    {
                        node.SetState(NodeState.Unlocked);
                    }
                    _nodeImage.color = new Color(0.8f, 0.75f, 0f);
                    break;
                case NodeState.Allocated:
                    _nodeImage.color = new Color(1f, 1f, 0f);
                    break;
            }
        }

        private void Update()
        {
            if (_state == NodeState.Unlocked)
            {
                if (_tree.Points > 0)
                {
                    _nodeImage.color = new Color(0.75f, 0.75f, 1f);
                }
                else
                {
                    _nodeImage.color = new Color(0.75f, 0.75f, 0f);
                }
            }
        }

        public Modifier Allocate()
        {
            _state = NodeState.Allocated;
            OnStateUpdated?.Invoke(_state);
            return _modifier;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                SelectNode();
            }
        }

        public void Attach(SkillNode node)
        {
            if (!_connections.ContainsKey(node))
            {
                var connection = _tree.CreateConnection(this, node);
                _connections.Add(node, connection);
                node.Attach(this, connection);
            }
        }
        
        private void Attach(SkillNode node, NodeConnection connection)
        {
            if (!_connections.ContainsKey(node))
            {
                _connections.Add(node, connection);
                node.Attach(this, connection);
            }
        }

        public void Detach(SkillNode node)
        {
            var connection = _connections[node];
            _connections.Remove(node);
            if (Application.isPlaying)
            {
                Destroy(connection.gameObject);
            }
            else
            {
                DestroyImmediate(connection.gameObject);
            }
            node.DetachData(this);
        }

        private void DetachData(SkillNode node)
        {
            _connections.Remove(node);
        }

        public bool HasConnection(SkillNode node)
        {
            return _connections.ContainsKey(node);
        }

        public void SetSkillType(SkillType type)
        {
            _nodeType = type;
            gameObject.name = type + " Node";
            var nodeParams = _tree.GetNode(type);
            if (nodeParams.HasValue)
            {
                var value = nodeParams.Value;
                _rectTransform.sizeDelta = Vector2.one * value.NodeSize;
                _nodeImage.material = value.NodeMaterial;
            }
        }

        public bool AddCluster(Cluster cluster)
        {
            if (_cluster != null)
            {
                return false;
            }

            _cluster = cluster;
            return true;
        }

        public void RemoveConnections()
        {
            var keys = _connections.Keys.ToArray();
            foreach (var node in keys)
            {
                node.Detach(this);
            }
            _connections.Clear();
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
