using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public enum LevelIndex
    {
        Menu,
        Level,
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
        private AsyncOperation _loadOp;
        private Action _loadComplete;

        private void Awake()
        {
            _levels = new Dictionary<LevelIndex, int>();
            foreach (var levelReference in _levelReferences)
            {
                _levels.Add(levelReference.Level, levelReference.SceneIndex);
            }
        }

        private void Update()
        {
            if (_loadOp is { isDone: true })
            {
                _loadOp = null;
                _loadComplete?.Invoke();
            }
        }

        public void LoadLevel(LevelIndex level, Action completed = null)
        {
            var levelIndex = _levels[level];
            
            if (SceneManager.GetSceneByBuildIndex(levelIndex).isLoaded)
            {
                completed?.Invoke();
                return;
            }
            
            _loadOp = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Additive);
            _loadOp.allowSceneActivation = true;
            _loadComplete = completed;
        }

        public void UnloadLevel(LevelIndex level, Action completed = null)
        {
            var levelIndex = _levels[level];
            
            if (!SceneManager.GetSceneByBuildIndex(levelIndex).isLoaded)
            {
                completed?.Invoke();
                return;
            }
            
            _loadOp = SceneManager.UnloadSceneAsync(levelIndex);
            _loadOp.allowSceneActivation = true;
            _loadComplete = completed;
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
