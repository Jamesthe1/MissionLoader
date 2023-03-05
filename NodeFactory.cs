using UnityEngine;
using System.Collections.Generic;

namespace MissionLoader {
    public static class NodeFactory {
        public static GameObject Create (NodeFactoryDatum nodeDatum, Transform parent = null) {
            string sortieName = nodeDatum.sortieTemplate.name;
            string nodeName = sortieName.Replace ("MISSION_", "NODE_");
            GameObject newNode = new GameObject (nodeName);
            newNode.transform.parent = parent;

            OverworldNode node = newNode.AddComponent<OverworldNode> ();
            node.sortieTemplate = nodeDatum.sortieTemplate;
            node.transform.position = nodeDatum.position;

            HUDIconOverworldNode hudNode = newNode.AddComponent<HUDIconOverworldNode> ();
            hudNode.mesh = Resources.Load<Mesh> ("ui/hudicons_landmarks_model_31");

            return newNode;
        }

        public static List<GameObject> CreateMultiple (List<NodeFactoryDatum> nodeDatums, Transform parent = null) {
            List<GameObject> nodes = new List<GameObject> ();
            nodeDatums.ForEach (nd => nodes.Add (Create (nd, parent)));
            return nodes;
        }
    }
}
