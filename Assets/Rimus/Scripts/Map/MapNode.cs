using System;

namespace Rimus.Scripts.Map
{
    [Serializable]
    public struct MapNode
    {
        public int Id;
        public MapNodeType Type;
        public MapNodeLidType LidType;
        public MapNodeStatus Status;
        public int[] NextNodeIds;
    }
}