using TMPro;
using UI.Elements.Skill_Tree;
using UnityEngine;

namespace UI.Components
{
    public class NodeDescription : MonoBehaviour
    {
        public struct DescriptionStrings
        {
            public string Name;
            public string Description;
        }
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _nodeName;
        [SerializeField] private TMP_Text _nodeDescription;
        [SerializeField] private float _boxOffset;
        
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
                    Rebuild(_nodeName.GetPreferredValues(description.Name),
                        _nodeDescription.GetPreferredValues(description.Description));
                }
                else
                {
                    _skillNode = null;
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnValidate()
        {
            _rectTransform = transform as RectTransform;
        }

        private void Rebuild(Vector2 prefName, Vector2 prefDesc)
        {
            var hs = _canvas.pixelRect.size * 0.5f;
            Vector2 position = _skillNode.transform.position;
            position -= hs;
            
            var width = Mathf.Max(prefName.x, prefDesc.x);
            var hw = width * 0.5f;
            var height = prefName.y + prefDesc.y;

            position.x = Mathf.Clamp(position.x, -hs.x + hw, hs.x - hw);
            
            position.y += _boxOffset;
            _rectTransform.anchoredPosition = position;
            _rectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}