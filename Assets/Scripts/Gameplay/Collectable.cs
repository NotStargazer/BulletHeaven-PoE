using System;
using UnityEngine;

namespace Gameplay
{
    public abstract class Collectable : MonoBehaviour, IPoolInstance<Collectable>
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        protected Player _player;

        protected Vector3 PlayerPosition => _player.transform.localPosition;
        protected Vector3 CollectablePosition => transform.localPosition;
        protected float Distance => Vector3.Distance(CollectablePosition, PlayerPosition);

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        protected abstract void OnDrop();
        
        public void Drop(Player player, Vector3 position)
        {
            _player = player;
            gameObject.SetActive(true);
            transform.localPosition = position;
            OnDrop();
        }

        protected abstract void OnCollect();
        
        protected void Collect()
        {
            OnCollect();
            gameObject.SetActive(false);
            Return(this);
        }
        
        public void OnRent()
        {
            
        }

        public Action<Collectable> Return { get; set; }
    }
}
