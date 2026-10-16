using Gameplay.Data;
using Global;
using UnityEngine;
using Utility;

namespace Gameplay
{
    public class LevelRoot : SingletonBehaviour<LevelRoot>
    {
        private static readonly int MAIN_TEX_ST = Shader.PropertyToID("_MainTex_ST");
        
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private RendererPropertyBlock _plane;
        [SerializeField] private EnemyList _enemyList;
        [SerializeField] private float _planeTextureScale;

        private Player _activePlayer;
        
        public override void Instantiate()
        {
            _enemyList.Instantiate();
            AllRegistries.Instance.Collectables.StartLevel();
        }

        private void LateUpdate()
        {
            //5 is the scale of unity plane mesh
            var localScale = new Vector3(_plane.transform.localScale.x, 1, _plane.transform.localScale.z) * 5;
            localScale *= 2 / _planeTextureScale;
            var playerPos = _activePlayer.Transform.position;
            _plane.transform.position = playerPos;
            _plane.SetProperty(MAIN_TEX_ST,
                new Vector4(
                    _planeTextureScale, _planeTextureScale,
                    playerPos.x / -localScale.x, playerPos.z / -localScale.z));
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

        public void DropItems(Vector3 position, int exp)
        {
            var experience = AllRegistries.Instance.Collectables.GetExperience(exp);
            experience.Drop(_activePlayer, position);
        }
    }
}
