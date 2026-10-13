using System.Globalization;
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

        public void TryUseSkill(IEntity user, Vector3 aimPosition)
        {
            _skill.TryUse(user, aimPosition, _supports);
        }

        public bool CanUse(IEntity user)
        {
            return _skill.CanUse(user, _supports);
        }

        public bool TrySocket(Support support)
        {
            return true;
        }

        public void ResetCooldown()
        {
            _skill.ResetCooldown();
        }
    }
}
