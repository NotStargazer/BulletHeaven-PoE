namespace Gameplay.Enemies.States
{
    public class ChaseState : IState
    {
        private readonly float _speedMultiplier;
        public ChaseState(float speedMultiplier)
        {
            _speedMultiplier = speedMultiplier;
        }

        public Enemy Subject { get; set; }

        public void Enter()
        {
        }

        public void Update()
        {
            var speed = (Subject as IEntity).StatTracker.GetMovementSpeed();
            Subject.Agent.speed = speed * _speedMultiplier;
            Subject.Agent.SetDestination(Subject.Player.Transform.position);
        }

        public void Exit()
        {
        }
    }
}