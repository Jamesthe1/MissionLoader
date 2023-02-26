using BepInEx;
using UnityEngine.SceneManagement;
using System.Collections;

namespace MissionLoader {
    /// <summary>
    /// Main plugin class
    /// </summary>
    [BepInPlugin (PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {

        private void Awake () {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Logger.LogInfo ($"Plugin awake done, waiting for scene to load");
        }

        private void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine (InitAfterFrame ()); // Need to wait a frame so that the scene can load
        }

        private IEnumerator InitAfterFrame () {
            yield return null;  // Wait 1 frame
            NodeSpawner.Spawn ();
            Logger.LogInfo ($"Created node spawner instance, initialization all done");
        }
    }
}
