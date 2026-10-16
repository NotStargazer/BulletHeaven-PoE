using TMPro;
using UI.Elements;
using UI.Localization;
using UnityEngine;

namespace UI.Components
{
    //I would love to make this generic since it's basically a duplicate of NodeDescription but my time is limited
    public class SkillDescription : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _skillName;
        [SerializeField] private TMP_Text _skillDescription;
        [SerializeField] private float _boxOffset;
        
        private SkillButton _skill;
        public SkillButton Skill
        {
            get => _skill;
            set
            {
                if (value)
                {
                    _skill = value;
                    var description = LocalizationManager.GetSkillDescription(value.Skill);
                    _skillName.SetText(description.Name);
                    _skillDescription.SetText(description.Description);
                    gameObject.SetActive(true);
                    Rebuild(_skillName.GetPreferredValues(description.Name),
                        _skillDescription.GetPreferredValues(description.Description));
                }
                else
                {
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
            Vector2 position = _skill.transform.position;
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