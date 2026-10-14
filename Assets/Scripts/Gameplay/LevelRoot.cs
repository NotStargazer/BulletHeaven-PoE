using System;
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
        }

        private void LateUpdate()
        {
            var localScale = _plane.transform.localScale;
            var scale = localScale / _planeTextureScale;
            var size = new Vector3(localScale.x * scale.x, 1, localScale.z * scale.z) * 2;
            var playerPos =_activePlayer.Transform.position;
            _plane.transform.position = playerPos;
            _plane.SetProperty(MAIN_TEX_ST, 
                new Vector4(
                    _planeTextureScale, _planeTextureScale,
                    playerPos.x / -size.x, playerPos.z / -size.x));
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
