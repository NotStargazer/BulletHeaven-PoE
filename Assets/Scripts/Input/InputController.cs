using System;
using Gameplay;
using UnityEngine;

namespace Input
{
    public enum InputChannel
    {
        Player,
        UI
    }

    public class InputController : MonoBehaviour
    {
        private PlayerInput _input;

        private void Awake()
        {
            _input = new PlayerInput();
        }

        public void AttachPlayer(Player player)
        {
            _input.Player.AddCallbacks(player);
            _input.Player.Enable();
        }
    }
}
