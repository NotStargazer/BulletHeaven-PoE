using UnityEngine;

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

        public void TryUseSkill(IEntity entity, Vector3 aimPosition)
        {
            _skill.TryUse(entity, aimPosition, _supports);
        }

        public bool TrySocket(Support support)
        {
            return true;
        }
    }
}
