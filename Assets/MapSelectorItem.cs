using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSelectorItem : MonoBehaviour
{
    [HideInInspector] public string map;
    [HideInInspector] public string mapTitle;
    [HideInInspector] public Sprite mapThumbnail;
    
    [SerializeField] private TextMeshProUGUI mapTitleTMP;
    [SerializeField] private Image mapThumbnailImg;

    private void Start()
    {
        mapThumbnailImg.sprite = mapThumbnail;
        mapTitleTMP.text = mapTitle;
    }
}
