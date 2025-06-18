using System.Collections.Generic;
using Global;
using Global.Registries;
using UnityEngine;
using Utility.Attributes;

namespace Gameplay
{
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private Transform _skillSpawnPosition;
        [SerializeField] [RegistryDropdown(typeof(SkillRegistry))] private string[] _defaultSkills;
        private List<SkillSlot> _skills;
        public Vector3 SkillSpawnPosition => _skillSpawnPosition.position;

        private int _skillIndex;
        private SkillSlot SelectedSkill => _skills[_skillIndex];

        private void Awake()
        {
            _skills = new List<SkillSlot>();
            foreach (var defaultSkill in _defaultSkills)
            {
                _skills.Add(new SkillSlot(AllRegistries.Instance.Skills.GetByID(defaultSkill).CreateInstance()));
            }
        }

        public void SwitchSkill(int direction)
        {
            _skillIndex += direction;
            _skillIndex %= _skills.Count;
            _skillIndex = _skillIndex < 0 ? _skills.Count - 1 : _skillIndex;
        }

        public void TryUseSkill(IEntity entity, Vector3 aimPosition)
        {
            if (_skills.Count == 0)
            {
                return;
            }
            
            SelectedSkill.TryUseSkill(entity, aimPosition);
        }
    }
}
