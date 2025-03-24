using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public enum LevelIndex
    {
        Test,
    }
    
    public class LevelManager : MonoBehaviour
    {
        [Serializable]
        private struct LevelReference
        {
#if UNITY_EDITOR
            public UnityEditor.SceneAsset SceneAsset;
#endif
            public LevelIndex Level;
            public int SceneIndex;
        }

        [SerializeField] private LevelReference[] _levelReferences;
        private Dictionary<LevelIndex, int> _levels;

        private void Awake()
        {
            _levels = new Dictionary<LevelIndex, int>();
            foreach (var levelReference in _levelReferences)
            {
                _levels.Add(levelReference.Level, levelReference.SceneIndex);
            }
        }

        public void LoadLevel(LevelIndex level, Action completed)
        {
            var levelIndex = _levels[level];
            
            if (SceneManager.GetSceneByBuildIndex(levelIndex).isLoaded)
            {
                completed();
                return;
            }
            
            var asyncOp = SceneManager.LoadSceneAsync(levelIndex);
            asyncOp.completed += operation =>
            {
                completed();
            };
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            for (var index = 0; index < _levelReferences.Length; index++)
            {
                var level = _levelReferences[index];
                if (level.SceneAsset)
                {
                    for (var i = 0; i < UnityEditor.EditorBuildSettings.scenes.Length; i++)
                    {
                        var buildScene = UnityEditor.EditorBuildSettings.scenes[i];
                        var sceneName = Path.GetFileNameWithoutExtension(buildScene.path);
                        if (sceneName == level.SceneAsset.name)
                        {
                            _levelReferences[index].SceneIndex = i;
                        }
                    }
                }
            }
#endif
        }
    }
}
