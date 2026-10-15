using System.Collections.Generic;
using Global;
using Global.Registries;
using UnityEngine;
using Utility;
using Utility.Attributes;

namespace Gameplay
{
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private InterfaceRef<IEntity> _entity;
        [SerializeField] private Transform _skillSpawnPosition;
        [SerializeField] [RegistryDropdown(typeof(SkillRegistry))] private string[] _defaultSkills;
        [SerializeField] private LayerMask _targetLayers;
        private List<SkillSlot> _skills;
        public Vector3 SkillSpawnPosition => _skillSpawnPosition.position;
        public LayerMask TargetLayers => _targetLayers;

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

        public void AddSkill(string skill)
        {
            _skills.Add(new SkillSlot(AllRegistries.Instance.Skills.GetByID(skill).CreateInstance()));
        }

        public void CycleSkill(int direction)
        {
            _skillIndex += direction;
            _skillIndex %= _skills.Count;
            _skillIndex = _skillIndex < 0 ? _skills.Count - 1 : _skillIndex;
        }

        public void TryUseSkill(Vector3 aimPosition)
        {
            if (_skills.Count == 0)
            {
                return;
            }
            
            SelectedSkill.TryUseSkill(_entity.I, aimPosition);
        }

        public void TryUseSkill(int index, Vector3 aimPosition)
        {
            if (_skills.Count == 0)
            {
                return;
            }
            
            _skills[Mathf.Clamp(index, 0, _skills.Count - 1)].TryUseSkill(_entity.I, aimPosition);
        }
        
        public bool CanUse()
        {
            return _skills.Count != 0 && SelectedSkill.CanUse(_entity.I);
        }

        public bool CanUse(int index)
        {
            return _skills.Count != 0 && _skills[Mathf.Clamp(index, 0, _skills.Count - 1)].CanUse(_entity.I);
        }

        public void ResetCooldowns()
        {
            foreach (var skillSlot in _skills)
            {
                skillSlot.ResetCooldown();
            }
        }
    }
}
