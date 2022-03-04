using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public GameObject hitEffect;
    [SerializeField] public GameObject hitIndicator;
    [SerializeField] public GameObject killIndicator;

    [HideInInspector] public float speed;
    [HideInInspector] public float damage;
    [HideInInspector] public float range;
    [HideInInspector] public Transform shooter;

    bool stop = false;

    void Start()
    {
        StartCoroutine(DestroyAtMaxRange());
    }

    void Update()
    {
        if (stop) { return; }

        transform.position += speed * Time.deltaTime * transform.forward;
    }

    IEnumerator DestroyAtMaxRange()
    {
        if(speed > 0) yield return new WaitForSeconds(range / speed);

        // wait until trail dissapears
        stop = true;
        Destroy(gameObject, .1f);
    }

    void OnMikeSphereTriggerEnter(RaycastHit hit)
    {

        // wait until trail dissapears
        stop = true;
        Destroy(gameObject, .1f);


        // WOW I DID NOT KNOW A METHOD LIKE VECTOR3/2.REFLECT EXISTS! THIS IS A LIFE SAVER!
        if(hitEffect != null) Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.forward, Vector3.Reflect(transform.TransformDirection(Vector3.forward), hit.normal)));


        // check for friendly fire
        if (hit.transform.root.CompareTag(tag) && hit.transform.gameObject.layer != 6) { return; }


        if(hit.transform.GetComponent<Health>() != null) hit.transform.GetComponent<Health>().TakeDamage(damage, shooter);
        else { return; }


        SpawnHitIndicator(hit.collider.gameObject);
    }

    #region hit indicator
    void SpawnHitIndicator(GameObject target)
    {
        if (target == null) return;
        if (hitIndicator == null) return;
        // There sould be a HitIndicatorManager but I'm to lazy to do that :/

        if (target.transform.GetComponent<Health>().health <= 0)
        {
            OnTargetKilled();
        }
        else
        {
            OnTargetHit();
        }
    }

    private void OnTargetHit()
    {
        Instantiate(hitIndicator, GameObject.Find("Canvas/HUD").transform);
    }

    private void OnTargetKilled()
    {
        Instantiate(killIndicator, GameObject.Find("Canvas/HUD").transform);
        shooter.root.GetComponent<Stats>().kills += 1;
    }

    #endregion
}
