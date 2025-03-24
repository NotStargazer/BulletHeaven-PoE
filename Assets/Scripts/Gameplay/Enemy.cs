using System;
using UnityEngine;

namespace Gameplay
{
    public abstract class Enemy : MonoBehaviour, IDamagable, IPoolInstance<Enemy>
    {
        private protected Player Target;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Spawn(Player player, Vector3 position)
        {
            Target = player;
            gameObject.SetActive(true);
            transform.localPosition = position;
        }

        protected abstract void OnDamaged();

        public void OnRent()
        {
            
        }

        public void Kill()
        {
            LevelRoot.Instance.DropItems(transform.localPosition);
            gameObject.SetActive(false);
            Return(this);
        }

        public Action<Enemy> Return { get; set; }


        public void ApplyDamage()
        {
            OnDamaged();
        }
    }
}
