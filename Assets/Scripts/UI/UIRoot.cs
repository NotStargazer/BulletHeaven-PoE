using Gameplay;
using Input;
using UI.Components;
using UnityEngine;

namespace UI
{
    public interface IUIRoot
    {
        CameraController GameCamera { get; }
        public void StartLevel();
        public void AddSkillPoint();
    }
    
    public class UIRoot : MonoBehaviour, IUIRoot
    {
        [SerializeField] private CameraController _gameCamera;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private SessionManager _sessionManager;
        [SerializeField] private DriverState _initialState;

        [Header("Components")] 
        [SerializeField] private PlayerStatsComponent _playerStats;
        [SerializeField] private SkillTree _skillTree;

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
                IEntity playerEntity = player;
                playerEntity.StatTracker.AddSource(_skillTree);
                _playerStats.SetObservables(player);
                _gameCamera.Attach(player.transform);
                _skillTree.Camera = _gameCamera;
                _inputController.AttachPlayer(player);
                _sessionManager.StartSession(this, player);
            });
        }

        public void AddSkillPoint()
        {
            _skillTree.AddPoint();
        }
    }
}
