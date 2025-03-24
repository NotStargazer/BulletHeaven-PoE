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

        public void TryUseSkill(Player player)
        {
            _skill.TryUse(player, _supports);
        }

        public bool TrySocket(Support support)
        {
            return true;
        }
    }
}
