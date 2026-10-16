using System;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class EndScreen : BaseMenu
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private TMP_Text _scoreStat;
        [SerializeField] private TMP_Text _timeStat;
        [SerializeField] private Button _returnToMenu;
        [SerializeField] private InputAction _menuAction;
        public Session Session { private get; set; }
        public IUIRoot Root { private get; set; }

        protected override void MenuAwake() { }

        private void Start()
        {
            _menuAction.performed += _ =>
            {
                Root.ReturnToMenu();
            };
            _returnToMenu.onClick.AddListener(Root.ReturnToMenu);
        }

        public void BindDeath(Player player)
        {
            IEntity entity = player;
            entity.HitPool.OnKill += () =>
            {
                if (!Visible)
                {
                    SetVisible(true);
                    _animation.Play("DeathAnimation");
                    _scoreStat.SetText(Mathf.FloorToInt(player.Experience).ToString());
                    var seconds = Mathf.FloorToInt(Session.TimePassed % 60);
                    var minutes = Mathf.FloorToInt(Session.TimePassed / 60);
                    _timeStat.SetText($"{minutes:00}:{seconds:00}");
                }
            };

        }

        public override void SetVisible(bool visible)
        {
            if (visible)
            {
                _menuAction.Enable();
            }
            else
            {
                _menuAction.Disable();
            }
            base.SetVisible(visible);
        }
    }
}