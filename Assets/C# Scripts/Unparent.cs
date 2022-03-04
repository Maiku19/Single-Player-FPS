using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unparent : MonoBehaviour
{
    [SerializeField] bool setPosition = false;
    [SerializeField] Vector3 positionOnUnparent = Vector3.zero;

    void Awake()
    {
        transform.SetParent(null);
        if(setPosition) transform.position = positionOnUnparent;
        Destroy(this);
    }
}
