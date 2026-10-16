using System;
using Global.Registries;
using UI.Elements;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utility.Attributes;

namespace UI.Components
{
    public class MainMenu : BaseMenu
    {
        [Serializable]
        private struct StartingSkill
        {
            public Sprite Sprite;
            [RegistryDropdown(typeof(SkillRegistry))] public string Skill;
        }
        
        [SerializeField] private SkillButton _skillButtonPrefab;
        [SerializeField] private SkillDescription _skillDescription;
        [SerializeField] private Transform _skillTransform;
        [SerializeField] private Button _quitButton;
        [SerializeField] private InputAction _scrollMenu;
        [SerializeField] private StartingSkill[] _skills;

        private SkillButton[] _buttons;
        private int _selectedSkill;
        public IUIRoot Root { private get; set; }
        
        protected override void MenuAwake()
        {
            _scrollMenu.performed += ctx =>
            {
                var val = ctx.ReadValue<Vector2>();
                
            };
            _scrollMenu.Enable();
            foreach (var startingSkill in _skills)
            {
                var button = Instantiate(_skillButtonPrefab, _skillTransform);
                button.BindSkill(startingSkill.Skill, startingSkill.Sprite);
                button.OnHover = s => _skillDescription.Skill = s;
                button.OnClick = () => OnClick(startingSkill.Skill);
            }
            
            _quitButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }

        private void OnClick(string skill)
        {
            Root.BeginGame(skill);
        }
    }
}