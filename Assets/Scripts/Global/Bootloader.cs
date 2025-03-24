using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
#endif

namespace Global
{
    public class Bootloader : MonoBehaviour
    {
        public static string BOOTLOADER_EDITOR_SCENE_PREF = "initialScene";

        [SerializeField] private MainEntryPoint _entryPoint;

        private AsyncOperation _sceneLoadOp;

        private IEnumerator Start()
        {
            if (!_entryPoint)
            {
                throw new ArgumentNullException(nameof(_entryPoint));
            }

            DontDestroyOnLoad(gameObject);
            gameObject.name = "[Bootloader]";

#if UNITY_EDITOR
            var path = EditorPrefs.GetString(BOOTLOADER_EDITOR_SCENE_PREF, null);
            path = string.IsNullOrEmpty(path) ? EditorBuildSettings.scenes[1].path : path;
            _sceneLoadOp = SceneManager.LoadSceneAsync(path, LoadSceneMode.Single);
            yield return new WaitUntil(InitialSceneLoadDone);
#else
            _sceneLoadOp = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            yield return new WaitUntil(InitialSceneLoadDone);
#endif
            //Main Entry Point
            var mainEntryPoint = Instantiate(_entryPoint.gameObject);
            mainEntryPoint.name = "[MainEntryPoint]";
            
            Destroy(gameObject);
        }

        private bool InitialSceneLoadDone()
        {
            return _sceneLoadOp.progress >= 1;
        }
    }
}
