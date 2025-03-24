using System;
using UnityEngine;

namespace Gameplay
{
    public class Collectable : MonoBehaviour, IPoolInstance<Collectable>
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private Player _player;
        private Vector2 _dropPosition;
        
        private Vector2 PlayerPosition => new(_player.transform.localPosition.x, _player.transform.localPosition.z);
        private float Distance => Vector2.Distance(_dropPosition, PlayerPosition);
        
        public void Drop(Player player)
        {
            var position = transform.localPosition;
            _dropPosition = new Vector2(position.x, position.z);
        }
        
        private void Collect()
        {
        }

        private void Update()
        {
            
        }

        public void OnRent()
        {
            
        }

        public Action<Collectable> Return { get; set; }
    }
}
