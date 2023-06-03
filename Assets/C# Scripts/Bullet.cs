using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public GameObject hitEffect;
    [SerializeField] public GameObject hitIndicator;
    [SerializeField] public GameObject killIndicator;
    [SerializeField] public GameObject _decal;

    [HideInInspector] public float speed;
    [HideInInspector] public float damage;
    [HideInInspector] public float range;
    [HideInInspector] public Transform shooter;

    void Start()
    {
        StartCoroutine(DestroyAtMaxRange());
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * transform.forward;
    }

    IEnumerator DestroyAtMaxRange()
    {
        if(speed > 0) yield return new WaitForSeconds(range / speed);

        // wait until trail dissapears
        Destroy(gameObject, .1f);
    }

    void OnMikeSphereTriggerEnter(RaycastHit hit)
    {
        // wait until trail dissapears
        Destroy(gameObject, .1f);

        speed = 0;
        transform.position = hit.point;


        // WOW I DID NOT KNOW A METHOD LIKE VECTOR3/2.REFLECT EXISTS! THIS IS A LIFE SAVER!
        if(hitEffect != null) Instantiate(hitEffect, transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.Reflect(transform.TransformDirection(Vector3.forward), hit.normal)));


        // check for friendly fire
        if (hit.transform.root.CompareTag(tag) && hit.transform.gameObject.layer != 6) { return; }

        if(_decal != null)
        {
            Instantiate(_decal, transform.position, transform.rotation);
        }

        if(hit.transform.TryGetComponent(out Health h)) h.TakeDamage(damage, shooter);
        else { return; }

        SpawnHitIndicator(hit.transform.gameObject);
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
