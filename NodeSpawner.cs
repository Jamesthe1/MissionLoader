using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace MissionLoader {
    /// <summary>
    /// Singleton that spawns nodes in the scene
    /// </summary>
    public class NodeSpawner : MonoBehaviour {
        private static string type = nameof(NodeSpawner);

        /// <summary>
        /// A function that must trigger when our scene is fully loaded
        /// </summary>
        public delegate void SceneReadyDelegate ();
        /// <summary>
        /// An event that must trigger when our scene is fully loaded
        /// </summary>
        public static event SceneReadyDelegate SceneReady;

        private static GameObject instance;
        private static List<NodeFactoryDatum> data = new List<NodeFactoryDatum> ();

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
        public static void ReadyNodes (List<NodeFactoryDatum> newNodes) {
            data.AddRange (newNodes);

            if (instance != null)
                instance.GetComponent<NodeSpawner> ().SpawnNodes ();
        }

        /// <summary>
        /// Finds objects from a list and readies them for spawning
        /// </summary>
        public static void FindAndReadyNodes (List<NamedNodeDatum> namedNodeData, List<Object> objects) {
            List<NodeFactoryDatum> nodes = new List<NodeFactoryDatum> ();
            foreach (NamedNodeDatum datum in namedNodeData) {
                SortieTemplate template = objects.FirstByName<SortieTemplate> (datum.sortieName);
                Transform nodePos = objects.FirstByName<GameObject> (datum.posName).transform;
                if (template == null || nodePos == null) {
                    string offender = template == null ? datum.sortieName : datum.posName;
                    Debug.LogError ($"{type}: Couldn't find " + offender);
                    continue;
                }

                nodes.Add (new NodeFactoryDatum (template, datum.connections, nodePos.position));
            }

            ReadyNodes (nodes);
        }

        // TODO: Add custom class that contains a sortietemplate class and a string list of node names

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
            Debug.Log ($"{type}: Spawning {data.Count} nodes");

            GameObject[] roots = SceneManager.GetActiveScene ().GetRootGameObjects ();
            GameObject galaxyRoot = roots.FirstOf (r => r.name == "#GALAXY_ROOT");
            // No need to check if galaxyRoot is missing as we can assure it will be there
            Transform nodeRoot = galaxyRoot.transform.FindChild ("GALAXY_ROTATOR/ROOT_Overworld");

            IEnumerable<OverworldNode> newNodes = NodeFactory.CreateMultiple (data, nodeRoot)
                                                             .Remap (g => g.GetComponent<OverworldNode> ());
            Debug.Log ($"{type}: Spawned {newNodes.Count ()} nodes");
            IEnumerable<string[]> connectionNames = data.Remap (d => d.connectionNames.ToArray ());
            Dictionary<OverworldNode, string[]> connections = newNodes.Zipper (connectionNames);
            foreach (KeyValuePair<OverworldNode, string[]> node in connections)
                AssignConnections (node, nodeRoot);
            
            Debug.Log ($"{type}: Connection process done");
            data.Clear ();    // Done with the list, prevent them from being recreated if this is called again
        }
    }
}
