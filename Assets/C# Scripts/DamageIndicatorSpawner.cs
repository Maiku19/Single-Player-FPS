using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorSpawner : MonoBehaviour
{
    public GameObject damageIndicator;
    public Transform spawnParent;

    public void SpawnIndicator(Transform target, Transform reciver)
    {
        DamageIndicator di = Instantiate(damageIndicator, spawnParent.position, spawnParent.rotation, spawnParent).GetComponent<DamageIndicator>().GetComponent<DamageIndicator>();
        di.target = target;
        di.reciver = reciver;
    }
}
