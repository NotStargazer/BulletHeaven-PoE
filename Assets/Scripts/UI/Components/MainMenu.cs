using System;
using Global.Registries;
using UI.Elements;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private InputAction _selectAction;
        [SerializeField] private StartingSkill[] _skills;

        private SkillButton[] _buttons;
        private int _selectedSkill;
        private bool _selectQuit;
        private bool _skip;
        public IUIRoot Root { private get; set; }
        
        protected override void MenuAwake()
        {
            _scrollMenu.started += ctx =>
            {
                var val = ctx.ReadValue<Vector2>();
                if (val.x != 0)
                {
                    if (_selectQuit)
                    {
                        return;
                    }
                    
                    _selectedSkill += Mathf.RoundToInt(val.x);
                    _selectedSkill %= _skills.Length;
                    _selectedSkill = _selectedSkill < 0 ? _skills.Length - 1 : _selectedSkill;
                    _skillDescription.Skill = _buttons[_selectedSkill];
                }
                if (val.y != 0)
                {
                    _selectQuit = !_selectQuit;
                    if (_selectQuit)
                    {
                        _skillDescription.Skill = null;
                        _quitButton.Select();
                    }
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        _skillDescription.Skill = _buttons[_selectedSkill];
                    }
                }
            };
            _selectAction.started += _ =>
            {
                if (_skip)
                {
                    _skip = false;
                    return;
                }
                if (_selectQuit)
                {
                    QuitGame();
                }
                else
                {
                    Root.BeginGame(_buttons[_selectedSkill].Skill);
                }
            };
            _buttons = new SkillButton[_skills.Length];
            for (var index = 0; index < _skills.Length; index++)
            {
                var startingSkill = _skills[index];
                var button = Instantiate(_skillButtonPrefab, _skillTransform);
                button.BindSkill(startingSkill.Skill, startingSkill.Sprite);
                button.OnHover = s => _skillDescription.Skill = s;
                button.OnClick = () => OnClick(startingSkill.Skill);
                _buttons[index] = button;
            }

            _quitButton.onClick.AddListener(QuitGame);
        }

        private void Start()
        {
            _skip = false;
        }

        private void OnClick(string skill)
        {
            Root.BeginGame(skill);
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public override void SetVisible(bool visible)
        {
            if (visible)
            {
                _scrollMenu.Enable();
                _selectAction.Enable();
                _skip = true;
            }
            else
            {
                _scrollMenu.Disable();
                _selectAction.Disable();
            }
            base.SetVisible(visible);
        }
    }
}