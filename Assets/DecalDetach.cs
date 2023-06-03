using UnityEngine;

public class DecalDetach : MonoBehaviour
{
    private void OnDestroy()
    {
        if (transform.parent == null) { return; }

        transform.parent = null;
    }
}
