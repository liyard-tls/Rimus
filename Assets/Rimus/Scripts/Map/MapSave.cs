using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rimus.Scripts.Map
{
    [Serializable]
    public class MapNodeLevelCollectionData
    {
        [SerializeField]
        private List<MapNode> _nodes = new List<MapNode>();
        
        public List<MapNode> Nodes => _nodes;
    }
}