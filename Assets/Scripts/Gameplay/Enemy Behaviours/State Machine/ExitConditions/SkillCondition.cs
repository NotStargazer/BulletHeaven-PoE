using Utility;

namespace Gameplay.Enemies.ExitConditions
{
    public class SkillCondition : IExitCondition
    {
        private readonly int _skillIndex;
        private readonly bool _canUse;
        private readonly float _range;
        
        public SkillCondition(int skillIndex, bool canUse, float range)
        {
            _skillIndex = skillIndex;
            _canUse = canUse;
            _range = range * range;
        }
        
        public Enemy Subject { get; set; }
        public void StateEntered() { }

        public bool ShouldExit()
        {
            var inRange = Subject.Player.Transform.position.SqrDistanceTo(Subject.Transform.position) < _range;
            var canUse = (Subject as IEntity).SkillManager.CanUse(_skillIndex);
            return inRange && !(_canUse ^ canUse);
        }
    }
}