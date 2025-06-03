namespace Gameplay
{
    public class SkillSlot
    {
        private Skill _skill;
        private Support[] _supports;

        public SkillSlot(Skill skill)
        {
            _skill = skill;
        }

        public void TryUseSkill(IEntity entity)
        {
            _skill.TryUse(entity, _supports);
        }

        public bool TrySocket(Support support)
        {
            return true;
        }
    }
}
