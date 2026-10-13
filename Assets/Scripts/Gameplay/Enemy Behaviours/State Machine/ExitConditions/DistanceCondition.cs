using Utility;

namespace Gameplay.Enemies.ExitConditions
{
    public class DistanceCondition : IExitCondition
    {
        private readonly float _distance;
        private readonly bool _within;
        
        public DistanceCondition(float distance, bool within)
        {
            _distance = distance * distance;
            _within = within;
        }

        public Enemy Subject { get; set; }
        public void StateEntered() { }

        public bool ShouldExit()
        {
            if (_within)
            {
                return Subject.Transform.position.SqrDistanceTo(Subject.Player.Transform.position) <= _distance;
            }
            
            return Subject.Transform.position.SqrDistanceTo(Subject.Player.Transform.position) >= _distance;
        }
    }
}