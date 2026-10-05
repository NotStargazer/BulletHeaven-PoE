using TMPro;
using UI.Elements.Skill_Tree;
using UnityEngine;

namespace UI.Components
{
    public class NodeDescription : MonoBehaviour
    {
        public struct ModifierDescription
        {
            public string Name;
            public string Description;
        }
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _nodeName;
        [SerializeField] private TMP_Text _nodeDescription;
        [SerializeField] private float _topBoxOffset;
        
        private SkillNode _skillNode;
        public SkillNode HoveredNode
        {
            get => _skillNode;
            set
            {
                if (value)
                {
                    _skillNode = value;
                    var description = value.GetModifierDescription();
                    _nodeName.SetText(description.Name);
                    _nodeDescription.SetText(description.Description);
                    gameObject.SetActive(true);
                }
                else
                {
                    _skillNode = null;
                    gameObject.SetActive(false);
                }
            }
        }

        private void Rebuild(Vector2 position)
        {
            var canvasHalf = _canvas.pixelRect.width * 0.5f;
            var height = _nodeName.preferredHeight + _nodeDescription.preferredHeight;
            var width = Mathf.Max(_nodeName.preferredWidth, _nodeDescription.preferredWidth);
            var hw = width * 0.5f;
            var lb = _rectTransform.anchoredPosition.x - hw;
            var rb = _rectTransform.anchoredPosition.x + hw;

            var borderOffsetLeft = Mathf.Min(lb, -canvasHalf);
            var borderOffsetRight = Mathf.Max(rb, canvasHalf);
            
            //TODO
            //_rectTransform.anchoredPosition
        }

        private void OnValidate()
        {
            _rectTransform = transform as RectTransform;
        }
    }
}