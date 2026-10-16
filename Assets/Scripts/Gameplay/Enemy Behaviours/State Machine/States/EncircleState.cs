using UI;
using UnityEngine;
using Utility;

namespace Gameplay.Enemies.States
{
    public class EncircleState : IState
    {
        private static readonly float PI2 = Mathf.PI * 2;
        
        private readonly float _radius;
        private readonly float _rotationSpeed;
        private readonly float _speed;
        private readonly float _catchupMultiplier;
        private readonly bool _facePlayer;
        private float _currentSpeedMult;
        private float _angle;
        private Transition _speedTransition;
        
        public EncircleState(float radius, float secondsPerRotation, float speed, float catchupMultiplier, bool facePlayer)
        {
            _radius = radius;
            _rotationSpeed = 1 / secondsPerRotation * PI2;
            _speed = speed;
            _catchupMultiplier = catchupMultiplier;
            _facePlayer = facePlayer;
            _speedTransition = new Transition(TransitionSpeed);
        }
        
        public Enemy Subject { get; set; }
        public void Enter()
        {
            var distance = Subject.Transform.position - Subject.Player.Transform.position;
            distance.Normalize();
            _angle = Mathf.Atan2(distance.z, distance.x);
            Subject.Agent.updateRotation = !_facePlayer;
            var speed = (Subject as IEntity).StatTracker.GetMovementSpeed();
            _currentSpeedMult = Subject.Agent.speed / speed;
            _speedTransition.StartTransition(0.1f, _currentSpeedMult, _speed);
        }

        public void Update()
        {
            var speed = (Subject as IEntity).StatTracker.GetMovementSpeed();
            var targetLocation = Subject.Player.Transform.position
                                 + new Vector3(Mathf.Cos(_angle), 0, Mathf.Sin(_angle)) * _radius;
            _angle = Mathf.Repeat(_angle + _rotationSpeed * Time.deltaTime, PI2);
            var distance = Vector3.Distance(Subject.Transform.position, targetLocation);
            Subject.Agent.speed = speed * _currentSpeedMult * Mathf.Max(distance * _catchupMultiplier, 1);
            Subject.Agent.SetDestination(targetLocation);
            if (_facePlayer)
            {
                var fwd = Subject.Transform.forward.ToXZ();
                var dir = (Subject.Player.Transform.position - Subject.Transform.position).ToXZ().normalized;
                Subject.Transform.forward = Vector2.SmoothDamp(fwd, dir, ref Subject.RotationVelocity, 0.1f, 10f).ToXZ3();
            }
            _speedTransition.Update();
        }

        public void Exit()
        {
            Subject.Agent.updateRotation = true;
        }
        
        private void TransitionSpeed(float speed)
        {
            _currentSpeedMult = speed;
        }
    }
}