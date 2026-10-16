using Gameplay.Data;
using Gameplay.Modifiers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using PlayerInput = Input.PlayerInput;

namespace Gameplay
{
    
    public class Player : MonoBehaviour, PlayerInput.IPlayerActions, IEntity
    {
        [SerializeField] private Transform _skillSpawnPosition;

        private ExperienceTracker _experienceTracker;
        private Vector2 _desiredDirection;
        [SerializeField] private StatTracker _statTracker;
        [SerializeField] private HitPool _hitPool;
        [SerializeField] private SkillManager _skillManager;
        [SerializeField] private ParticleSystem _deathVFX;
        [SerializeField] private GameObject _model;
        
        [SerializeField] private int _levelUpRequirement;
        [SerializeField] private float _levelUpGrowthFactor;
        
        public ObservableValue<float> Experience;
        public ObservableValue<float> ExperiencePercentage;
        private bool _isDead;
        
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
                var aim = context.ReadValue<Vector2>();
                var worldPosition = UIRoot.GameCamera.GetWorldFloor(aim);
                if (!worldPosition.HasValue)
                {
                    return;
                }

                var direction = (worldPosition - transform.localPosition).Value.normalized;
                Transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            
        }

        public void OnAimController(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var aim = context.ReadValue<Vector2>();
                if (aim != Vector2.zero)
                {
                    Transform.rotation = Quaternion.LookRotation(UIRoot.GameCamera.WorldRotation * aim.ToXZ3(), Vector3.up);
                }
            }
        }

        public void OnToggleSkills(InputAction.CallbackContext context)
        {
            
        }
        
        private void OnLevelUp(int level)
        {
            UIRoot.AddSkillPoint();
        }

        private void Awake()
        {
            _experienceTracker = new ExperienceTracker(_levelUpRequirement, _levelUpGrowthFactor,
                (e, p) =>
                {
                    Experience.Value = e;
                    ExperiencePercentage.Value = p;
                }, OnLevelUp);
            Experience = new ObservableValue<float>(0);
            ExperiencePercentage = new ObservableValue<float>(0);
            _hitPool.OnKill += () =>
            {
                if (!_isDead)
                {
                    _isDead = true;
                    _model.SetActive(false);
                    _deathVFX.Play(true);
                }
            };
        }

        private void Start()
        {
            _hitPool.Restore();
        }

        private void Update()
        {
            if (!_hitPool.Alive)
            {
                return;
            }
            
            if (_desiredDirection.magnitude > 0)
            {
                transform.Translate(
                    UIRoot.GameCamera.WorldRotation * _desiredDirection.ToXZ3() *
                    (_statTracker.GetMovementSpeed() * Time.deltaTime),
                    Space.World);
            }

            Vector3 aimPosition;
            if (Physics.SphereCast(Transform.position, 4, Transform.forward, 
                    out var hit, 20, _skillManager.TargetLayers))
            {
                aimPosition = hit.transform.position;
            }
            else
            {
                aimPosition = Transform.position + Transform.forward * 15;
            }

            aimPosition.y = _skillManager.SkillSpawnPosition.y;
            
            _skillManager.TryUseSkill(aimPosition);
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
