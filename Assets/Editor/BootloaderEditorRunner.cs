using Global;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Game.Editor
{
    [InitializeOnLoad]
    public class BootloaderEditorRunner
    {
        static BootloaderEditorRunner()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange mode)
        {
            if (mode != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }
            
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            var currentScene = EditorSceneManager.GetActiveScene().path;
            var bootloaderScene = EditorBuildSettings.scenes[0].path;
            EditorPrefs.SetString(Bootloader.BOOTLOADER_EDITOR_SCENE_PREF,
                currentScene != bootloaderScene ? currentScene : null);
            var bootloaderSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootloaderScene);
            EditorSceneManager.playModeStartScene = bootloaderSceneAsset;
        }
    }
}
