using Gameplay;
using Input;
using UnityEngine;

namespace UI
{
    public interface IUIRoot
    {
        CameraController GameCamera { get; }
        public void StartLevel();
    }
    
    public class UIRoot : MonoBehaviour, IUIRoot
    {
        [SerializeField] private CameraController _gameCamera;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private SessionManager _sessionManager;
        [SerializeField] private DriverState _initialState;

        public CameraController GameCamera => _gameCamera;

        private GameDriver _gameDriver;
        private InputController _inputController;
        
        public void StartGame(InputController inputControllerInstance)
        {
            _inputController = inputControllerInstance;
            _gameDriver = new GameDriver(this, _initialState);
        }

        public void StartLevel()
        {
            _levelManager.LoadLevel(LevelIndex.Test, () =>
            {
                var player = LevelRoot.Instance.StartLevel(_sessionManager.GetCharacter(0));
                player.UIRoot = this;
                _gameCamera.Attach(player.transform);
                _inputController.AttachPlayer(player);
                
                _sessionManager.StartSession(this, player);
            });
        }

    }
}
