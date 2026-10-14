using UI;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Utility;

namespace Gameplay.Enemies.States
{
    public class AttackState : IState
    {
        private static readonly int COLOR = Shader.PropertyToID("_Color");
        
        private readonly float _attackTime;
        private readonly int _skillIndex;
        private readonly float _turnTime;
        private readonly float _maxTurnSpeed;
        private readonly TransitionFunction _attackDelay;
        private Vector2 _rotationVelocity;
        
        public AttackState(float attackTime, int skillIndex, float turnTime, float maxTurnSpeed)
        {
            _attackTime = attackTime;
            _skillIndex = skillIndex;
            _turnTime = turnTime;
            _maxTurnSpeed = maxTurnSpeed;
            _attackDelay = new TransitionFunction(Wind);
        }
        
        public Enemy Subject { get; set; }
        
        public void Enter()
        {
            _rotationVelocity = Vector2.zero;
            _attackDelay.StartTransition(_attackTime, 0, 1, easeFunc: Easing.InCubic, onComplete: UseSkill);
            Subject.Agent.speed = 0;
        }

        public void Update()
        {
            var fwd = Subject.Transform.forward.ToXZ();
            var dir = (Subject.Player.Transform.position - Subject.Transform.position).ToXZ().normalized;
            Subject.Transform.forward = Vector2.SmoothDamp(fwd, dir, ref _rotationVelocity, _turnTime, _maxTurnSpeed).ToXZ3();
        }

        public void Exit()
        {
        }

        private void Wind(float t)
        {
            Subject.Renderer.SetProperty(COLOR, Color.Lerp(Color.white, Color.lightCoral, t));
        }

        private void UseSkill()
        {
            IEntity entity = Subject;
            if (entity.HitPool.Alive)
            {
                entity.SkillManager.TryUseSkill(_skillIndex, Subject.Player.Transform.position);
            }
            _attackDelay.StartTransition(_attackTime, 1, 0, easeFunc: Easing.InCubic);
        }
    }
}