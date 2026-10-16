using System;
using System.Collections.Generic;
using Gameplay.Modifiers;
using TMPro;
using UI.Elements.Skill_Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Components
{
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

        [SerializeField] private GameObject _controllerCursor;
        [SerializeField] private TMP_Text _availablePoints;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _openButton;
        [SerializeField] private List<SkillNode> _rootNodes;
        [SerializeField] private NodeDescription _nodeDescription;
        [SerializeField] private ZoomableScrollRect _scrollView;
        [SerializeField] private RectTransform _connectionTransform;
        [SerializeField] private SkillNode _skillNodePrefab;
        [SerializeField] private NodeConnection _connectionPrefab;
        [SerializeField] private InputAction _closeAction;
        [SerializeField] private InputAction _toggleAction;
        [SerializeField] private InputAction _zoomAction;
        [SerializeField] private InputAction _scrollAction;
        [SerializeField] private InputAction _allocateAction;
        [SerializeField] private NodeParameters[] _nodeParameters;

        private List<SkillNode> _selectedNodes;
        private int _allocatedPoints;
        private int _totalSkillPoints;
        private float _ctrlZoom;
        private Vector2 _ctrlScroll;
        private List<RaycastResult> _raycasts;
        private PointerEventData _eventData;
        

        public SkillNode HoveredNode
        {
            get => _nodeDescription.HoveredNode;
            set => _nodeDescription.HoveredNode = value;
        }

        public int Points => _totalSkillPoints - (_allocatedPoints + _selectedNodes.Count);

        public void ResetSkillTree()
        {
            _totalSkillPoints = 0;
            _availablePoints.SetText(Points.ToString());
            HoveredNode = null;
            foreach (var skillNode in _rootNodes)
            {
                skillNode.ResetState(true);
            }
        }
        
        protected override void MenuAwake()
        {
            _raycasts = new List<RaycastResult>();
            _eventData = new PointerEventData(EventSystem.current);
            _toggleAction.performed += _ =>
            {
                if (Visible)
                {
                    SetVisible(false);
                }
                else if (Points > 0)
                {
                    SetVisible(true);
                }
            };
            _toggleAction.Enable();
            _closeAction.performed += _ =>
            {
                SetVisible(false);
            };
            _zoomAction.performed += ctx =>
            {
                _ctrlZoom = ctx.ReadValue<Vector2>().y;
            };
            _zoomAction.canceled += _ =>
            {
                _ctrlZoom = 0;
            };
            _scrollAction.performed += ctx =>
            {
                _ctrlScroll = ctx.ReadValue<Vector2>();
            };
            _scrollAction.canceled += _ =>
            {
                _ctrlScroll = Vector2.zero;
            };
            _allocateAction.performed += _ =>
            {
                if (HoveredNode && HoveredNode.State is NodeState.Unlocked or NodeState.Selected)
                {
                    HoveredNode.SetState(NodeState.Selected);
                    _selectedNodes.Add(HoveredNode);
                    AllocateNodes();
                }
            };
            _allocateAction.Enable();
            _selectedNodes = new List<SkillNode>();
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

            if (_allocatedPoints + _selectedNodes.Count < _totalSkillPoints)
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
                _totalSkillPoints--;
                AddModifier(node.Allocate());
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

        public override void SetVisible(bool visible)
        {
            Time.timeScale = visible ? 0 : 1;
            if (visible)
            {
                _closeAction.Enable();
                _scrollAction.Enable();
                _zoomAction.Enable();
            }
            else
            {
                _closeAction.Disable();
                _scrollAction.Disable();
                _zoomAction.Disable();
            }
            base.SetVisible(visible);
        }

        private void Update()
        {
            if (_ctrlScroll != Vector2.zero)
            {
                _controllerCursor.SetActive(true);
                _scrollView.Scroll(_ctrlScroll * Time.unscaledDeltaTime);
                var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                _eventData.position = screenCenter;
                EventSystem.current.RaycastAll(_eventData, _raycasts);
                HoveredNode = null;
                foreach (var raycastResult in _raycasts)
                {
                    if (raycastResult.gameObject.TryGetComponent(out SkillNode node))
                    {
                        HoveredNode = node;
                    }
                }
            }

            if (_ctrlZoom != 0)
            {
                _scrollView.Zoom(_zoomAction.ReadValue<Vector2>().y * Time.unscaledDeltaTime * 20);
            }
        }

        public Action<Modifier> AddModifier { get; set; }
        public Action<Modifier> RemoveModifier { get; set; }
    }
}
