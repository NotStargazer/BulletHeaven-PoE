using Global;
using UnityEngine;
using Utility;

namespace Gameplay
{
    public class LevelRoot : SingletonBehaviour<LevelRoot>
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private EnemyList _enemyList;

        private Player _activePlayer;
        
        public override void Instantiate()
        {
            _enemyList.Instantiate();
        }

        public Player StartLevel(Player player)
        {
            _activePlayer = GameObject.Instantiate(player, _spawnPoint.position, Quaternion.identity, transform);
            return _activePlayer;
        }

        public bool TryGetEnemyInstances(Session session, out Enemy[] enemy)
        {
            enemy = _enemyList.GetEnemyInstances(session);
            return enemy != null;
        }

        public void DropItems(Vector3 position)
        {
            var experience = AllRegistries.Instance.Collectables.GetExperience(10);
            experience.Drop(_activePlayer, position);
        }
    }
}
