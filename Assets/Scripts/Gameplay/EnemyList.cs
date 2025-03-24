using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    [Serializable]
    public record EnemyWave
    {
        [Serializable]
        public record EnemyInstance
        {
            public Enemy EnemyPrefab;
            public int SpawnWeight;
            public int MaxCount;
        }
        
        public EnemyInstance[] EnemyInstances;
        [Range(0, 100)] public float SpawnTime;
        [Range(0, 100)] public float Frequency;

        private float _spawnTime;
        
        public int GetSpawnCount(Session session)
        {
            _spawnTime -= Frequency * Time.deltaTime;
            var count = 0;
            while (_spawnTime <= 0)
            {
                _spawnTime += 1;
                count++;
            }

            return count;
        }
        
        private int TotalWeight
        {
            get
            {
                var weight = 0;
                foreach (var enemyInstance in EnemyInstances)
                {
                    weight += enemyInstance.SpawnWeight;
                }

                return weight;
            }
        }

        private int? _calculatedTotalWeight;
        private Queue<EnemyInstance> _cachedEnemyInstances;

        public EnemyInstance RollEnemy()
        {
            if (EnemyInstances.Length == 0)
            {
                throw new Exception($"WAVE AT TIME {SpawnTime} IS EMPTY");
            }
            
            if (EnemyInstances.Length == 1)
            {
                return EnemyInstances[0];
            }
            
            _cachedEnemyInstances ??= new Queue<EnemyInstance>();
            _calculatedTotalWeight ??= TotalWeight;
            _cachedEnemyInstances.Clear();

            foreach (var enemyInstance in EnemyInstances)
            {
                _cachedEnemyInstances.Enqueue(enemyInstance);
            }

            var remainingWeight = _calculatedTotalWeight.Value;
            
            while (_cachedEnemyInstances.Count > 1)
            {
                var instance = _cachedEnemyInstances.Dequeue();
                var roll =  Random.value * remainingWeight;
                if (roll <= instance.SpawnWeight)
                {
                    return instance;
                }
            }

            return _cachedEnemyInstances.Dequeue();
        }
    }
    
    [Serializable]
    public class EnemyList
    {
        [SerializeField] private EnemyWave[] _enemyWaves;
        
        private Dictionary<Enemy, InstancePool<Enemy>> _enemyPools;

        public Enemy[] GetEnemyInstances(Session session)
        {
            foreach (var enemyWave in _enemyWaves)
            {
                if (session.PercentageTimePassed >= enemyWave.SpawnTime)
                {
                    var count = enemyWave.GetSpawnCount(session);
                    if (count == 0)
                    {
                        return null;
                    }

                    var enemies = new Enemy[count];
                    for (var i = 0; i < count; i++)
                    {
                        var enemyInstance = enemyWave.RollEnemy();
                        var pool = _enemyPools[enemyInstance.EnemyPrefab];
                        if (pool.TotalActive < enemyInstance.MaxCount)
                        {
                            enemies[i] = pool.Rent();
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return enemies;
                }
            }

            return null;
        }

        public void Instantiate()
        {
            _enemyPools = new Dictionary<Enemy, InstancePool<Enemy>>();
            
            foreach (var enemyWave in _enemyWaves)
            {
                foreach (var enemyInstance in enemyWave.EnemyInstances)
                {
                    _enemyPools.TryAdd(
                        enemyInstance.EnemyPrefab,
                        new InstancePool<Enemy>(enemyInstance.EnemyPrefab, 30, 10, 200));
                }
            }
        }
    }
}
