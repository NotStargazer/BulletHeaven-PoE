using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Gameplay.Modifiers;
using Global;
using Global.Registries;
using UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility.Attributes;

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
        [SerializeField] private SkillType _nodeType;
        [SerializeField] private SkillTree _tree;
        [SerializedDictionary("Adjacent Node", "Connection")]
        [SerializeField] private SerializedDictionary<SkillNode, NodeConnection> _connections;
        [SerializeField] private Image _nodeImage;
        [RegistryDropdown(typeof(SkillTreeModifierRegistry))]
        [SerializeField] private string _treeModifier;
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

        private void Awake()
        {
            _modifier = AllRegistries.Instance.SkillTreeModifiers.GetByID(_treeModifier).CreateInstance();
            _nodeImage.sprite = _modifier.Icon;
        }

        private void Update()
        {
            if (_state == NodeState.Unlocked)
            {
                var col = _nodeImage.color;
                col.b = _tree.Points > 0 ? 1f : 0f;
                _nodeImage.color = col;
            }
        }
        
        private void SelectNode()
        {
            if (_state is NodeState.Locked or NodeState.Allocated)
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
                    _nodeImage.color = new Color(0.5f, 0f, 0f);
                    break;
                case NodeState.Unlocked:
                    foreach (var node in AdjacentNodes)
                    {
                        if (node._state <= NodeState.Unlocked)
                        {
                            node.SetState(NodeState.Locked);
                        }
                    }
                    _nodeImage.color = new Color(0.75f, 1f, 0f);
                    break;
                case NodeState.Selected:
                    foreach (var node in AdjacentNodes)
                    {
                        if (node._state == NodeState.Locked)
                        {
                            node.SetState(NodeState.Unlocked);
                        }
                    }
                    _nodeImage.color = new Color(0.9f, 0.75f, 0f);
                    break;
                case NodeState.Allocated:
                    _nodeImage.color = new Color(1f, 1f, 0f);
                    break;
            }
        }

        public Modifier Allocate()
        {
            SetState(NodeState.Allocated);
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

        public void SetModifier(Modifier modifier)
        {
            
        }
    }
}
