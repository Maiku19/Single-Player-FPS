using UnityEngine;

public class ExplosiveBarrel : Health
{
    [SerializeField] private float maxDamage;
    [SerializeField] private float range;

    public override void Die(GameObject killer)
    {
        Explode(killer);

        Instantiate(deathFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Explode(GameObject killer)
    {
        Collider[] players = Physics.OverlapSphere(transform.position, range);
        DealDamage(ref players, killer);
    }

    void DealDamage(ref Collider[] players, GameObject killer)
    {
        foreach (Collider player in players)
        {
            if(player.TryGetComponent(out Stats playerStats) && playerStats.teamId != killer.transform.root.GetComponent<Stats>().teamId && player.GetComponent<Health>() != null)
            {
                Vector3 barrelClosestPos = gameObject.GetComponent<Collider>().ClosestPoint(player.transform.position);
                Vector3 playerClosestPos = player.ClosestPoint(transform.position);
                float ammount = Mathf.Clamp01(Vector3.Distance(barrelClosestPos, playerClosestPos) / range) * maxDamage;

                player.GetComponent<Health>().TakeDamage(ammount, transform);
            }
        }
    }
}
