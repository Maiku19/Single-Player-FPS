using UnityEngine;

public class ExplosiveBarrel : Health
{
    [SerializeField] private float maxDamage;
    [SerializeField] private float range;
    [SerializeField] Collider _collider;

    bool _isDead = false;
    public override void Die(GameObject killer)
    {
        if(_isDead) return;
        _isDead = true;

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
        foreach (Collider hit in players)
        {
            if(hit == _collider) { continue; }

            if(hit.TryGetComponent(out Player player))
            {
                if (player.Team != killer.transform.root.GetComponent<Stats>().teamId)
                {
                    Damage(hit, player.Health);
                }
            }
            else if(hit.TryGetComponent(out Health h))
            {
                Damage(hit, h);
            }
        }

        void Damage(Collider hit, Health health)
        {
            Vector3 barrelClosestPos = _collider.ClosestPoint(hit.transform.position);
            Vector3 playerClosestPos = hit.ClosestPoint(transform.position);
            float ammount = Mathf.Clamp01(Vector3.Distance(barrelClosestPos, playerClosestPos) / range) * maxDamage;

            health.TakeDamage(ammount, transform);
        }
    }
}
