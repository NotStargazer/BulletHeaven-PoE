using System;
using Gameplay.Data;
using Gameplay.Modifiers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Input.PlayerInput;

namespace Gameplay
{
    
    public class Player : MonoBehaviour, PlayerInput.IPlayerActions, IEntity
    {
        [SerializeField] private Transform _skillSpawnPosition;

        private ExperienceTracker _experienceTracker;
        private Vector2 _desiredDirection;
        private Vector2 _lastScreenAimPosition;
        [SerializeField] private StatTracker _statTracker;
        [SerializeField] private HitPool _hitPool;
        [SerializeField] private SkillManager _skillManager;
        
        [SerializeField] private int _levelUpRequirement;
        [SerializeField] private float _levelUpGrowthFactor;

        public Transform Transform => transform;

        StatTracker IEntity.StatTracker
        {
            get => _statTracker;
            set => _statTracker = value;
        }

        HitPool IEntity.HitPool
        {
            get => _hitPool;
            set => _hitPool = value;
        }

        SkillManager IEntity.SkillManager
        {
            get => _skillManager;
            set => _skillManager = value;
        }

        public IUIRoot UIRoot { get; set; }

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
                _lastScreenAimPosition = context.ReadValue<Vector2>();
            }
        }

        public void OnToggleSkills(InputAction.CallbackContext context)
        {
            
        }

        private void OnExperienceGain(int experience, float percentageExperience)
        {
            UIRoot.SetExperience(percentageExperience);
        }
        
        private void OnLevelUp(int level)
        {
            UIRoot.AddSkillPoint();
        }

        private void Awake()
        {
            _experienceTracker = new ExperienceTracker(_levelUpRequirement, _levelUpGrowthFactor, OnExperienceGain, OnLevelUp);
        }

        private void Start()
        {
            _hitPool.Restore();
        }

        private void Update()
        {
            if (_desiredDirection.magnitude > 0)
            {
                transform.Translate(
                    UIRoot.GameCamera.WorldRotation *
                    new Vector3(_desiredDirection.x, 0, _desiredDirection.y) *
                    (_statTracker.GetMovementSpeed() * Time.deltaTime),
                    Space.World);
            }
            
            var worldPosition = UIRoot.GameCamera.GetWorldFloor(_lastScreenAimPosition);
            if (!worldPosition.HasValue)
            {
                return;
            }

            var direction = (worldPosition - transform.localPosition).Value.normalized;
            transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
            
            _skillManager.TryUseSkill(worldPosition.Value);
        }

        public void GrantExperience(int experience)
        {
            _experienceTracker.AddExperience(experience);
        }
        
        private void OnValidate()
        {
            (this as IEntity).ImplementComponents();
        }
    }
}
