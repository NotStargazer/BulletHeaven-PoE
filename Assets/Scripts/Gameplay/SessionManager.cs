using UI;
using UnityEngine;
using Utility;

namespace Gameplay
{
    public record Session
    {
        public Session(float warmupTime, float sessionLength)
        {
            _startTime = Time.time - warmupTime;
            _endTime = _startTime + sessionLength + warmupTime;
            _active = true;
        }
        
        private readonly float _startTime;
        private readonly float _endTime;
        private bool _active;

        public float Duration => _endTime - _startTime;
        public float TimeRemaining => Mathf.Max(0, _endTime - Time.time);
        public float TimePassed => Mathf.Min(Duration, Time.time - _startTime);
        public float PercentageTimePassed => Duration / TimePassed * 100;
        public float PercentageTimeRemaining => 100 - PercentageTimePassed;
        public bool Active => Time.time >= _startTime && Time.time <= _endTime && _active;

        public void End()
        {
            _active = false;
        }
    }
    
    public class SessionManager : MonoBehaviour
    {
        [SerializeField] private Player[] _characterList;

        private Player _player;
        private Session _currentSession;
        private IUIRoot _uiRoot;
        public string StartingSkill { private get; set; }

        public Player GetCharacter(int index)
        {
            return _characterList[index];
        }

        public void StartSession(IUIRoot uiRoot, Player player)
        {
            _player = player;
            ((IEntity)player).SkillManager.AddSkill(StartingSkill);
            _uiRoot = uiRoot;
            _currentSession = new Session(2, TimeScaler.Minutes(20));
        }
        
        public Session EndSession()
        {
            _currentSession.End();
            return _currentSession;
        }

        private void Update()
        {
            if (_currentSession is { Active: true } && LevelRoot.HasInstance)
            {
                if (LevelRoot.Instance.TryGetEnemyInstances(_currentSession, out var enemies))
                {
                    foreach (var enemy in enemies)
                    {
                        Vector3 pos;
                        while (!_uiRoot.GameCamera.TryGetRandomBorderPosition(out pos)) { }
                        enemy.Spawn(_player, pos);
                    }

                }
            }
        }
    }
}
