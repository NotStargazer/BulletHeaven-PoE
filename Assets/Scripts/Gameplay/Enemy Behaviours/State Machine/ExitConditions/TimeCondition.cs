using UnityEngine;

namespace Gameplay.Enemies.ExitConditions
{
    public class TimeCondition : IExitCondition
    {
        private readonly float _duration;
        private float _endTime;
        
        public TimeCondition(float duration)
        {
            _duration = duration;
        }

        public Enemy Subject { get; set; }
        public void StateEntered()
        {
            _endTime = Time.time + _duration;
        }

        public bool ShouldExit()
        {
            return Time.time >= _endTime;
        }
    }
}