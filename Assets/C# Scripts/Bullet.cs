using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public GameObject hitEffect;
    [SerializeField] public GameObject hitIndicator;
    [SerializeField] public GameObject killIndicator;
    [SerializeField] public GameObject _decal;

    [HideInInspector] public float Speed;
    [HideInInspector] public float Damage;
    [HideInInspector] public float Range;
    [HideInInspector] public Player Shooter;

    void Start()
    {
        StartCoroutine(DestroyAtMaxRange());
    }

    private void Update()
    {
        transform.position += Speed * Time.deltaTime * transform.forward;
    }

    IEnumerator DestroyAtMaxRange()
    {
        if(Speed > 0) yield return new WaitForSeconds(Range / Speed);

        // wait until trail disappears
        Destroy(gameObject, .1f);
    }

    void OnMikeSphereTriggerEnter(RaycastHit hit)
    {
        // wait until trail disappears
        Destroy(gameObject, .1f);

        Speed = 0;
        transform.position = hit.point;

        // WOW I DID NOT KNOW A METHOD LIKE VECTOR3/2.REFLECT EXISTS! THIS IS A LIFE SAVER!
        if(hitEffect != null) Instantiate(hitEffect, transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.Reflect(transform.TransformDirection(Vector3.forward), hit.normal)));

        // check for friendly fire
        if (hit.transform.root.CompareTag(tag) && hit.transform.gameObject.layer != 6) { return; }

        if(hit.transform.TryGetComponent(out Health h)) h.TakeDamage(Damage, Shooter.transform);
        else { return; }


        OnHit(hit.transform.gameObject);
    }

    #region hit indicator
    void OnHit(GameObject target)
    {
        // There should be a HitIndicatorManager but I'm to lazy to do that :/
        if (target == null) return;

        if (_decal != null)
        {
            Instantiate(_decal, transform.position, transform.rotation);
        }

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
        if (!Shooter.IsBot) Instantiate(hitIndicator, GameObject.Find("Canvas/HUD").transform);
    }

    private void OnTargetKilled()
    {
        if(!Shooter.IsBot) Instantiate(killIndicator, GameObject.Find("Canvas/HUD").transform);
        Shooter.Stats.Kills++;
    }

    #endregion
}
