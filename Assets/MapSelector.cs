using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [System.Serializable]
    public struct Map
    {
        public string mapName;
        public string mapDisplayName;
        public Sprite mapThumbnail;
    }

    [SerializeField] private GameObject mapTemplate;
    [SerializeField] private Map[] maps;
    [SerializeField] private float padding = 10;

    public string selectedMap;

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
            mapItem.map = item.mapName;
            mapItem.mapThumbnail = item.mapThumbnail;
            mapItem.mapTitle = item.mapDisplayName;
        }

        mapTemplate.SetActive(false);
    }

    public void SelectMap(MapSelectorItem map)
    {
        selectedMap = map.map;
    }
}
