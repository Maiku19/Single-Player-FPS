using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [System.Serializable]
    public struct Map
    {
        public string mapID;
        public string mapDisplayName;
        public Sprite mapThumbnail;
        public Sprite MapPreview;

        public Map(string mapName, string mapDisplayName, Sprite mapThumbnail, Sprite mapPreview)
        {
            this.mapID = mapName;
            this.mapDisplayName = mapDisplayName;
            this.mapThumbnail = mapThumbnail;
            MapPreview = mapPreview;
        }
    }

    [SerializeField] private GameObject mapTemplate;
    [SerializeField] private Map[] maps;
    [SerializeField] private float padding = 10;

    public Map SelectedMap { get => _selectedMap ??= maps[0]; set => _selectedMap = value; }
    Map? _selectedMap;

    private void Start()
    {
        RectTransform content = GetComponentInChildren<ScrollRect>().content;

        for (int i = 0; i < maps.Length; i++)
        {
            Map item = maps[i];

            content.sizeDelta += (mapTemplate.GetComponent<RectTransform>().rect.xMax + padding) * Vector2.right;
            GameObject map = Instantiate(mapTemplate, content);

            map.name = item.mapDisplayName;
            map.transform.localPosition = (map.GetComponent<RectTransform>().rect.xMax + padding) * i * Vector2.right;
            
            MapSelectorItem mapItem = map.GetComponent<MapSelectorItem>();
            mapItem.map = item.mapID;
            mapItem.mapThumbnail = item.mapThumbnail;
            mapItem.mapTitle = item.mapDisplayName;
        }

        mapTemplate.SetActive(false);
    }

    public void SelectMap(MapSelectorItem map)
    {
        SelectedMap = new(map.map, map.mapTitle, map.mapThumbnail, map.MapPreviewImage);
    }
}
