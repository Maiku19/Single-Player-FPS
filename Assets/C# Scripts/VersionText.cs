using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    [SerializeField] private string prefix = "v";

    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = prefix + Application.version;
    }
}
