using System;
using System.IO;
using Rimus.Scripts.Tools;
using UnityEngine;

namespace Rimus.Scripts.Map
{
    public class MapSaver : MonoBehaviour
    {
        [SerializeField]
        private string _resourcesPath = "Map/MapData";

        [SerializeField]
        private string _saveFileName = "map-save.json";

        private string SavePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        
        public MapNodeLevelCollectionData LoadMap()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    string savedJson = File.ReadAllText(SavePath);
                    return JsonUtility.FromJson<MapNodeLevelCollectionData>(savedJson);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to load saved map data from path '{SavePath}': {ex.Message}");
                }
            }

            TextAsset jsonAsset = Resources.Load<TextAsset>(_resourcesPath);
            if (jsonAsset == null)
            {
                Log.Error($"Failed to load map data from path: {_resourcesPath}");
                return null;
            }

            try
            {
                return JsonUtility.FromJson<MapNodeLevelCollectionData>(jsonAsset.text);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to parse map data: {ex.Message}");
                return null;
            }
        }
        
        public void SaveMap(MapNodeLevelCollectionData mapData)
        {
            try
            {
                string json = JsonUtility.ToJson(mapData, true);
                File.WriteAllText(SavePath, json);
                Log.Info($"Map data saved to '{SavePath}'");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save map data to path '{SavePath}': {ex.Message}");
            }
        }
    }
}
