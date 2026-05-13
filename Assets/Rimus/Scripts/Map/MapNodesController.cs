using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rimus.Scripts.Map
{
    public class MapNodesController : MonoBehaviour
    {
        [SerializeField] private List<MapNodeView> _nodesViews = new List<MapNodeView>();

        
        
        [Button]
        public void UpdateNodeViews()
        {
            foreach (var node in _nodesViews)
            {
                node.UpdateView();
            }
        }
    }
}