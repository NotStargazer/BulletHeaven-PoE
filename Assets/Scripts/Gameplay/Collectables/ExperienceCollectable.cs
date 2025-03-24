using UnityEngine;

namespace Gameplay.Collectables
{
    public class ExperienceCollectable : Collectable
    {
        [SerializeField] private float _initialVelocity;
        [SerializeField] private float _acceleration;
        
        private float _speed;
        private bool _collecting;
        public int Experience { private get; set; }

        protected override void OnDrop()
        {
            _speed = _initialVelocity;
            _collecting = false;
        }

        protected override void OnCollect()
        {
            _player.GrantExperience(Experience);
        }
        
        private void Update()
        {
            if (_collecting)
            {
                var speed = _speed * Time.deltaTime;
                var velocity = (PlayerPosition - CollectablePosition).normalized * speed;
                transform.Translate(velocity, Space.Self);
                _speed += _acceleration * Time.deltaTime;

                if (speed >= Distance)
                {
                    Collect();
                }
                
                return;
            }
            
            if (Distance <= _player.Stats.PickupRange)
            {
                _collecting = true;
            }
        }
    }
}
