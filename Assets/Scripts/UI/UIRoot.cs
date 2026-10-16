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
        public void EndLevel();
        public void SetMainMenuVisibility(bool visible);
        public void AddSkillPoint();
        public void BeginGame(string skill);
        public void ReturnToMenu();
    }
    
    public class UIRoot : MonoBehaviour, IUIRoot
    {
        [SerializeField] private CameraController _gameCamera;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private SessionManager _sessionManager;
        [SerializeField] private DriverState _initialState;

        [Header("Components")] 
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private PlayerStatsComponent _playerStats;
        [SerializeField] private SkillTree _skillTree;
        [SerializeField] private EndScreen _endScreen;

        public CameraController GameCamera => _gameCamera;

        private GameDriver _gameDriver;
        private InputController _inputController;

        public void StartGame(InputController inputControllerInstance)
        {
            _mainMenu.Root = this;
            _endScreen.Root = this;
            _inputController = inputControllerInstance;
            _gameDriver = new GameDriver(this, _initialState);
        }

        public void StartLevel()
        {
            _levelManager.LoadLevel(LevelIndex.Level, () =>
            {
                _skillTree.ResetSkillTree();
                _playerStats.ResetStats();
                var player = LevelRoot.Instance.StartLevel(_sessionManager.GetCharacter(0));
                player.UIRoot = this;
                IEntity playerEntity = player;
                playerEntity.StatTracker.AddSource(_skillTree);
                playerEntity.HitPool.OnKill += () =>
                {
                    _endScreen.Session = _sessionManager.EndSession();
                };
                _playerStats.gameObject.SetActive(true);
                _playerStats.SetObservables(player);
                _gameCamera.Attach(player.transform);
                _inputController.AttachPlayer(player);
                _endScreen.BindDeath(player);
                _sessionManager.StartSession(this, player);
            });
        }

        public void EndLevel()
        {
            _skillTree.SetVisible(false);
            _endScreen.SetVisible(false);
            _playerStats.gameObject.SetActive(false);
        }

        public void SetMainMenuVisibility(bool visible)
        {
            _mainMenu.SetVisible(visible);
        }
        
        public void AddSkillPoint()
        {
            _skillTree.AddPoint();
        }

        public void BeginGame(string skill)
        {
            _sessionManager.StartingSkill = skill;
            _gameDriver.ChangeState(DriverState.Game);
        }

        public void ReturnToMenu()
        {
            _levelManager.UnloadLevel(LevelIndex.Level);
            _gameDriver.ChangeState(DriverState.Menu);
        }
    }
}
