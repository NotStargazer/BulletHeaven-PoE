using System;
using Gameplay.Data;
using Global;
using Global.Registries;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility.Attributes;
using PlayerInput = Input.PlayerInput;

namespace Gameplay
{
    [Serializable]
    public struct PlayerStats
    {
        public float MovementSpeed;
        public float PickupRange;
    }
    
    public class Player : MonoBehaviour, PlayerInput.IPlayerActions
    {
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private Transform _skillSpawnPosition;

        [SerializeField] [RegistryDropdown(typeof(SkillRegistry))] private string _startingSkill;

        private ExperienceTracker _experienceTracker;
        private SkillSlot _skillSlot;
        private Vector2 _desiredDirection;
        
        public IUIRoot UIRoot { set; private get; }
        public Vector3 SkillSpawnPosition => _skillSpawnPosition.position;
        public PlayerStats Stats => _playerStats;

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                _desiredDirection = context.ReadValue<Vector2>();
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var value = context.ReadValue<Vector2>();
                var worldPosition = UIRoot.GameCamera.GetWorldFloor(value);

                if (!worldPosition.HasValue)
                {
                    return;
                }
                
                var direction = (worldPosition - transform.localPosition).Value.normalized;
                transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        public void OnToggleSkills(InputAction.CallbackContext context)
        {
            
        }

        private void OnLevelUp(int level)
        {
            
        }

        private void Awake()
        {
            _experienceTracker = new ExperienceTracker(100, 1.1f, OnLevelUp);
            _skillSlot = new SkillSlot(AllRegistries.Instance.Skills.GetByID(_startingSkill).CreateInstance());
        }

        private void Update()
        {
            if (_desiredDirection.magnitude > 0)
            {
                transform.Translate(UIRoot.GameCamera.WorldRotation * new Vector3(_desiredDirection.x, 0, _desiredDirection.y) * (_playerStats.MovementSpeed * Time.deltaTime), Space.World);
            }
            
            _skillSlot.TryUseSkill(this);
        }

        public void GrantExperience(int experience)
        {
            _experienceTracker.AddExperience(experience);
        }
    }
}
