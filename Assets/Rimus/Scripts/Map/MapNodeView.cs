using System;
using System.Collections.Generic;
using Rimus.Scripts.Tools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Rimus.Scripts.Map
{
    [Serializable]
    public struct ConnectionData
    {
        public MapNodeView From;
        public MapNodeView To;
        public UIConnectionGraphic Graphic;
        
        public ConnectionData(MapNodeView from, MapNodeView to, UIConnectionGraphic graphic)
        {
            From = from;
            To = to;
            Graphic = graphic;
        }
    }

    [Serializable]
    public struct ConnectionAnchors
    {
        public RectTransform Top;
        public RectTransform Bottom;
        public RectTransform Left;
        public RectTransform Right;
    }
    
    public class MapNodeView : MonoBehaviour
    {
        public MapNode Node => _node;
        public ConnectionAnchors ConnectionAnchors => _connectionAnchors;
        public List<ConnectionData> Connections => _connections;
        
        [SerializeField] private MapNode _node;
        
        [SerializeField] private MapNodeAssets _assets;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _lidImage;
        [SerializeField] private Image _lidIconImage;
        [SerializeField] private ConnectionAnchors _connectionAnchors;
        [SerializeField] private List<ConnectionData> _connections = new List<ConnectionData>();

        public void Initialize(MapNode node)
        {
            UpdateBackground(node);
            UpdateIcons(node);
            UpdateLid(node);
        }

        private void UpdateBackground(MapNode node)
        {
            _backgroundImage.sprite = node.Status switch
            {
                MapNodeStatus.Default => _assets.DraftedBackground,
                MapNodeStatus.Current => _assets.FilledBackground,
                MapNodeStatus.Passed => _assets.FilledBackground,
                MapNodeStatus.Failed => _assets.FilledBackground,
                _ => _assets.DraftedBackground
            };
            _backgroundImage.color = node.Status switch
            {
                MapNodeStatus.Default => _assets.DefaultColor,
                MapNodeStatus.Current => _assets.CurrentColor,
                MapNodeStatus.Passed => _assets.PassedColor,
                MapNodeStatus.Failed => _assets.FailedColor,
                _ => _assets.DefaultColor
            };
        }

        private void UpdateIcons(MapNode node)
        {
            switch (node.Type)
            {
                case MapNodeType.Monster:
                    _iconImage.sprite = _assets.MonsterIcon;
                    _iconImage.color = _assets.MonsterColor;
                    break;
                case MapNodeType.Human:
                    _iconImage.sprite = _assets.HumanIcon;
                    _iconImage.color = _assets.HumanColor;
                    break;
                case MapNodeType.Boss:
                    _iconImage.sprite = _assets.BossIcon;
                    _iconImage.color = _assets.BossColor;
                    break;
                case MapNodeType.Heal:
                    _iconImage.sprite = _assets.HealIcon;
                    _iconImage.color = _assets.HealColor;
                    break;
                case MapNodeType.Treasure:
                    _iconImage.sprite = _assets.TreasureIcon;
                    _iconImage.color = _assets.TreasureColor;
                    break;
                default:
                    _iconImage.sprite = null;
                    _iconImage.color = Color.clear;
                    break;
            }
        }

        private void UpdateLid(MapNode node)
        {
            _lidImage.sprite = _assets.Lid;
            _lidImage.gameObject.SetActive(node.Status == MapNodeStatus.Default);
            switch (node.LidType)
            {
                case MapNodeLidType.Question:
                    _lidIconImage.sprite = _assets.QuestionIcon;
                    _lidIconImage.color = _assets.QuestionColor;
                    break;
                case MapNodeLidType.Exclamation:
                    _lidIconImage.sprite = _assets.ExclamationIcon;
                    _lidIconImage.color = _assets.ExclamationColor;
                    break;
                default:
                    _lidIconImage.sprite = null;
                    _lidIconImage.color = Color.clear;
                    break;
            }
        }

        public void SetConnectionAnchors(ConnectionAnchors anchors)
        {
            _connectionAnchors = anchors;
        }
    }
}