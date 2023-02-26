using UnityEngine;
using System.Collections.Generic;

namespace MissionLoader {
    /// <summary>
    /// A template for building nodes in <see cref="NodeFactory"/>, mainly used in <see cref="NodeSpawner.ReadyNodes(List{NodeFactoryDatum})"/>
    /// </summary>
    public class NodeFactoryDatum {
        public SortieTemplate sortieTemplate;
        public List<string> connectionNames;
        public Vector3 position;

        public NodeFactoryDatum (SortieTemplate sortieTemplate, List<string> connectionNames, Vector3 position) {
            this.sortieTemplate = sortieTemplate;
            this.connectionNames = connectionNames;
            this.position = position;
        }
    }
}
