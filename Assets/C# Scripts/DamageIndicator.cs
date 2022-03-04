using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public Transform target;
    public Transform reciver;
    public Transform targetSeeker;
    Vector3 position;

    private void Start()
    {
        position = target.position;
    }

    void Update()
    {
        Vector2 targetVectorRelativeToPlayer = new Vector2(position.x - reciver.position.x, position.z - reciver.position.z);

        float lookDirY = Mathf.Atan2(targetVectorRelativeToPlayer.normalized.x, targetVectorRelativeToPlayer.normalized.y) * Mathf.Rad2Deg;

        transform.localEulerAngles = new Vector3(0, 0, reciver.eulerAngles.y - lookDirY);
    }
}
