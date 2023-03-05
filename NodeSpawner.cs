using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace MissionLoader {
    /// <summary>
    /// Singleton that spawns nodes in the scene
    /// </summary>
    public class NodeSpawner : MonoBehaviour {
        /// <summary>
        /// A function that must trigger when our scene is fully loaded
        /// </summary>
        public delegate void SceneReadyDelegate ();
        /// <summary>
        /// An event that must trigger when our scene is fully loaded
        /// </summary>
        public static event SceneReadyDelegate SceneReady;

        private static GameObject instance;
        private static List<NodeFactoryDatum> datums = new List<NodeFactoryDatum> ();

        /// <summary>
        /// The singleton instance of this object, if spawned
        /// </summary>
        public static GameObject Instance {
            get { return instance; }
        }

        public static void Spawn () {
            if (instance != null) return;
            
            SceneReady?.Invoke ();
            instance = new GameObject ("NodeSpawner", typeof (NodeSpawner));
        }

        /// <summary>
        /// Readies nodes to be spawned in mission select
        /// </summary>
        /// <param name="newNodes"></param>
        public static void ReadyNodes (List<NodeFactoryDatum> newNodes) {
            datums.AddRange (newNodes);

            if (instance != null)
                instance.GetComponent<NodeSpawner> ().SpawnNodes ();
        }

        private void Awake () {
            SpawnNodes ();
        }

        private void AssignConnections (KeyValuePair<OverworldNode, string[]> node, Transform root) {
            foreach (string connectionName in node.Value) {
                Transform other = root.FindChild (connectionName);
                if (other != null)
                    node.Key.ConnectTo (other.GetComponent<OverworldNode> ());
            }
        }

        private void SpawnNodes () {
            string type = nameof (NodeSpawner);
            Debug.Log ($"{type}: Spawning {datums.Count} nodes");

            GameObject[] roots = SceneManager.GetActiveScene ().GetRootGameObjects ();
            GameObject galaxyRoot = roots.FirstOf (r => r.name == "#GALAXY_ROOT");
            // No need to check if galaxyRoot is missing as we can assure it will be there
            Transform nodeRoot = galaxyRoot.transform.FindChild ("GALAXY_ROTATOR/ROOT_Overworld");

            IEnumerable<OverworldNode> newNodes = NodeFactory.CreateMultiple (datums, nodeRoot)
                                                             .Remap (g => g.GetComponent<OverworldNode> ());
            Debug.Log ($"{type}: Spawned {newNodes.Count ()} nodes");
            IEnumerable<string[]> connectionNames = datums.Remap (d => d.connectionNames.ToArray ());
            Dictionary<OverworldNode, string[]> connections = newNodes.Zipper (connectionNames);
            foreach (KeyValuePair<OverworldNode, string[]> node in connections)
                AssignConnections (node, nodeRoot);
            
            Debug.Log ($"{type}: Connection process done");
            datums.Clear ();    // Done with the list, prevent them from being recreated if this is called again
        }
    }
}
