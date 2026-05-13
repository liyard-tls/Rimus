#if UNITY_EDITOR
using System.Collections.Generic;
using Rimus.Scripts.Tools;
using Rimus.Scripts.Tools.UI;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Rimus.Scripts.Map
{
    /// <summary>
    /// Unity Editor class for building map connections. It gathers all MapNodeView components in children and builds connections between them based on their NextNodeIds.
    /// </summary>
    public class MapBuilder : MonoBehaviour
    {
        [SerializeField] private List<MapNodeView> _nodesViews = new List<MapNodeView>();
        
        private Dictionary<int, MapNodeView> _nodeViews = new Dictionary<int, MapNodeView>();
        
        [Button]
        public void UpdateNodes()
        {
            _nodeViews = new Dictionary<int, MapNodeView>();
            
            foreach (var nodeView in _nodesViews)
            {
                if (!_nodeViews.TryAdd(nodeView.Node.Id, nodeView))
                {
                    Log.Error($"Duplicate node id: {nodeView.Node.Id}");
                    continue;
                }
            }
        }

        [Button]
        public void RebuildConnections()
        {
            Log.Info("=== Rebuilding map connections... ===");
            foreach (var node in _nodeViews.Values)
            {
                RebuildConnectionsForNode(node);
            }
            Log.Info("=== Finished rebuilding map connections ===");
        }

        [Button]
        public void SetAnchorsToView()
        {
            // temporary method to set anchors to view for all nodes, should be removed after anchors will be set in prefab
            foreach (var node in _nodeViews.Values)
            {
                Undo.RecordObject(node, "Set Connection Anchors");

                var anchors = node.ConnectionAnchors;
                anchors.Top = node.transform.Find("TopAnchor").GetComponent<RectTransform>();
                anchors.Bottom = node.transform.Find("BottomAnchor").GetComponent<RectTransform>();
                anchors.Left = node.transform.Find("LeftAnchor").GetComponent<RectTransform>();
                anchors.Right = node.transform.Find("RightAnchor").GetComponent<RectTransform>();
                node.SetConnectionAnchors(anchors);

                EditorUtility.SetDirty(node);
                EditorSceneManager.MarkSceneDirty(node.gameObject.scene);
            }
        }

        public void RebuildConnectionsForNode(MapNodeView nodeView)
        {
            Log.Info($"Rebuilding connections for node {nodeView.Node.Id}...");
            // destroy old connections
            foreach (var connection in nodeView.Connections)
            {
                if(connection.Graphic != null)
                    DestroyImmediate(connection.Graphic.gameObject);
            }

            // clear connections list
            nodeView.Connections.Clear();
            
            Log.Info($"Cleared connections for node {nodeView.Node.Id}...");

            // build new connections
            foreach (var nextNodeId in nodeView.Node.NextNodeIds)
            {
                if (_nodeViews.TryGetValue(nextNodeId, out var nextNodeView))
                {
                    CreateConnectionGraphic(nodeView, nextNodeView);
                }
            }
        }

        private void CreateConnectionGraphic(MapNodeView nodeView, MapNodeView nextNodeView)
        {
            Undo.RecordObject(nodeView, "Create Map Connection");

            var connectionGraphic = UIConnectionGraphic.Create(nodeView.transform);
            Undo.RegisterCreatedObjectUndo(connectionGraphic.gameObject, "Create Map Connection");

            var connectionAnchors = GetConnectionAnchors(nodeView, nextNodeView);
            connectionGraphic.SetNodes(connectionAnchors.from, connectionAnchors.to);
            connectionGraphic.SetMode(UIConnectionGraphic.ConnectionMode.Solid);
            nodeView.Connections.Add(new ConnectionData(nodeView, nextNodeView, connectionGraphic));

            EditorUtility.SetDirty(nodeView);
            EditorUtility.SetDirty(connectionGraphic);
            EditorSceneManager.MarkSceneDirty(nodeView.gameObject.scene);
            Log.Info($"Created connection from node {nodeView.Node.Id} to node {nextNodeView.Node.Id}");
        }
        
        private (RectTransform from, RectTransform to) GetConnectionAnchors(MapNodeView fromNode, MapNodeView toNode)
        {
            var fromAnchors = new[]
            {
                fromNode.ConnectionAnchors.Top,
                fromNode.ConnectionAnchors.Bottom,
                fromNode.ConnectionAnchors.Left,
                fromNode.ConnectionAnchors.Right
            };

            var toAnchors = new[]
            {
                toNode.ConnectionAnchors.Top,
                toNode.ConnectionAnchors.Bottom,
                toNode.ConnectionAnchors.Left,
                toNode.ConnectionAnchors.Right
            };

            RectTransform nearestFromAnchor = null;
            RectTransform nearestToAnchor = null;
            var shortestDistanceSqr = float.MaxValue;

            foreach (var fromAnchor in fromAnchors)
            {
                foreach (var toAnchor in toAnchors)
                {
                    if (fromAnchor == null || toAnchor == null)
                    {
                        continue;
                    }

                    var distanceSqr = (fromAnchor.position - toAnchor.position).sqrMagnitude;
                    if (distanceSqr >= shortestDistanceSqr)
                    {
                        continue;
                    }

                    shortestDistanceSqr = distanceSqr;
                    nearestFromAnchor = fromAnchor;
                    nearestToAnchor = toAnchor;
                }
            }

            return (nearestFromAnchor, nearestToAnchor);
        }
    }
}
#endif
