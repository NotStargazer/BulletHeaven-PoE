using UI;

namespace Gameplay.Enemies.States
{
    public class ChaseState : IState
    {
        private readonly float _speedMultiplier;
        private readonly float _speedSmoothingDuration;
        private float _currentSpeedMult;
        private readonly TransitionFunction _speedTransition;
        public ChaseState(float speedMultiplier, float speedSmoothingDuration)
        {
            _currentSpeedMult = speedMultiplier;
            _speedMultiplier = speedMultiplier;
            _speedSmoothingDuration = speedSmoothingDuration;
            _speedTransition = new TransitionFunction(TransitionSpeed);
        }

        public Enemy Subject { get; set; }

        public void Enter()
        {
            var speed = (Subject as IEntity).StatTracker.GetMovementSpeed();
            _currentSpeedMult = Subject.Agent.speed / speed;
            _speedTransition.StartTransition(_speedSmoothingDuration, _currentSpeedMult, _speedMultiplier);
        }

        public void Update()
        {
            var speed = (Subject as IEntity).StatTracker.GetMovementSpeed();
            Subject.Agent.speed = speed * _currentSpeedMult;
            Subject.Agent.SetDestination(Subject.Player.Transform.position);
        }

        public void Exit() { }

        private void TransitionSpeed(float speed)
        {
            _currentSpeedMult = speed;
        }
    }
}