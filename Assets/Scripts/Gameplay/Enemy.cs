using System;
using Gameplay.Data;
using Gameplay.Modifiers;
using UnityEngine;

namespace Gameplay
{
    public abstract class Enemy : MonoBehaviour, IEntity, IPoolInstance<Enemy>
    {
        private protected Player Target;
        [SerializeField] private StatTracker _statTracker;
        [SerializeField] private HitPool _hitPool;
        [SerializeField] private SkillManager _skillManager;

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

        private void Awake()
        {
            gameObject.SetActive(false);
            _hitPool.OnKill += Kill;
        }

        public void Spawn(Player player, Vector3 position)
        {
            Target = player;
            transform.localPosition = position;
            gameObject.SetActive(true);
            _hitPool.Restore();
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
            Validate();
        }

        public virtual void Validate()
        {
            (this as IEntity).ImplementComponents();
        }
    }
}
