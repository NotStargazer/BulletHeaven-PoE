using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Elements
{
    public class SkillButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _skillIcon;

        public Action OnClick;
        public Action<SkillButton> OnHover;
        public string Skill { get; private set; }

        public void BindSkill(string skill, Sprite sprite)
        {
            _skillIcon.sprite = sprite;
            Skill = skill;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnHover(null);
        }
    }
}