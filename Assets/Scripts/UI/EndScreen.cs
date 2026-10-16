using System;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EndScreen : BaseMenu
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private TMP_Text _scoreStat;
        [SerializeField] private TMP_Text _timeStat;
        [SerializeField] private Button _returnToMenu;
        public Session Session { private get; set; }
        public IUIRoot Root { private get; set; }

        protected override void MenuAwake() { }

        private void Start()
        {
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
                    _scoreStat.SetText(player.Experience.ToString());
                    var seconds = Mathf.FloorToInt(Session.TimePassed % 60);
                    var minutes = Mathf.FloorToInt(Session.TimePassed / 60);
                    _timeStat.SetText($"{minutes:00}:{seconds:00}");
                }
            };

        }
    }
}