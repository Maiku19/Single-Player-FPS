using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private bool killOnEnter = true;
    [SerializeField] private bool killOnExit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!killOnEnter) { return; }

        if (other.GetComponent<Health>() != null) { other.GetComponent<Health>().TakeDamage(Mathf.Infinity, transform); }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!killOnExit) { return; }

        if(other.GetComponent<Health>() != null) { other.GetComponent<Health>().TakeDamage(Mathf.Infinity, transform); }
    }
}
