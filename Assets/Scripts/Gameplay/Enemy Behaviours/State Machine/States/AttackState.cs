using UI;

namespace Gameplay.Enemies.States
{
    public class AttackState : IState
    {
        private readonly float _attackTime;
        private readonly int _skillIndex;
        private readonly TransitionFunction _attackDelay;
        public AttackState(float attackTime, int skillIndex)
        {
            _attackTime = attackTime;
            _skillIndex = skillIndex;
            _attackDelay = new TransitionFunction(Windup);
        }
        
        public Enemy Subject { get; set; }
        
        public void Enter()
        {
            _attackDelay.StartTransition(_attackTime, 0, 1, onComplete: UseSkill);
            Subject.Agent.speed = 0;
        }

        public void Update()
        {
        }

        public void Exit()
        {
        }

        private void Windup(float t)
        {
            
        }

        private void UseSkill()
        {
            var sm = (Subject as IEntity).SkillManager;
            sm.TryUseSkill(_skillIndex, Subject.Player.Transform.position);
        }
    }
}