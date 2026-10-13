using System;
using System.Globalization;
using Gameplay.Data;
using Gameplay.Enemies;
using Gameplay.Modifiers;
using UnityEngine;
using UnityEngine.AI;
using Utility;

namespace Gameplay
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour, IEntity, IPoolInstance<Enemy>
    {
        [SerializeField] private FlexibleStateMachine _stateMachine;
        [SerializeField] private StatTracker _statTracker;
        [SerializeField] private HitPool _hitPool;
        [SerializeField] private SkillManager _skillManager;
        [SerializeField] private NavMeshAgent _agent;

        public Transform Transform => transform;
        public NavMeshAgent Agent => _agent;
        public Player Player { get; private set; }

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

        private void Awake()
        {
            gameObject.SetActive(false);
            _hitPool.OnKill += Kill;
        }

        private void Start()
        {
            _stateMachine.Construct(this);
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        public void Spawn(Player player, Vector3 position)
        {
            Player = player;
            transform.localPosition = position;
            gameObject.SetActive(true);
            _hitPool.Restore();
            _skillManager.ResetCooldowns();
        }

        public void Kill()
        {
            LevelRoot.Instance.DropItems(transform.localPosition);
            gameObject.SetActive(false);
            Return(this);
        }

        public Action<Enemy> Return { get; set; }
        
        
        private void OnValidate()
        {
            _agent = GetComponent<NavMeshAgent>();
            (this as IEntity).ImplementComponents();
        }

    //     private void OnGUI()
    //     {
    //         var mcam = Camera.main;
    //         var distance = Player.Transform.position.SqrDistanceTo(Transform.position);
    //         var pos = mcam.WorldToScreenPoint(Transform.position);
    //         pos.y = Screen.height - pos.y;
    //         pos.y -= 50;
    //         GUI.Label(new Rect(pos, new Vector2(100, 20)), distance.ToString(CultureInfo.InvariantCulture),
    //             new GUIStyle { normal = new GUIStyleState { textColor = Color.red }, fontSize = 20});
    //         pos.y -= 22;
    //         GUI.Label(new Rect(pos, new Vector2(100, 20)), _stateMachine.CurrentState,
    //             new GUIStyle { normal = new GUIStyleState { textColor = Color.red }, fontSize = 20});
    //         pos.y -= 22;
    //         GUI.Label(new Rect(pos, new Vector2(100, 20)), _skillManager.CanUse().ToString(),
    //             new GUIStyle { normal = new GUIStyleState { textColor = Color.red }, fontSize = 20});
    //     }
     }
}
