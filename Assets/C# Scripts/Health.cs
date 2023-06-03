using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    public float health = 0;
    public float healthPerSec = 50;
    public float healingDelay = 5;
    public Slider healthBar;
    public DamageIndicatorSpawner dis;
    public GameObject deathFX;

    protected bool _isDead = false;
    Coroutine _healing;

    void Awake()
    {
        health = maxHealth;
        if (healthBar != null) healthBar.maxValue = maxHealth;
    }

    public void TakeDamage(float damage, Transform damager)
    {
        health -= damage;
        if (healthBar != null) healthBar.value = health;
        if (dis != null) { dis.SpawnIndicator(damager, GetComponent<Player>()); }

        // Heal over time
        if(_healing != null) StopCoroutine(_healing);
        _healing = StartCoroutine(Heal(healingDelay));

        if (health <= 0)
        {
            Die(damager.gameObject);
        }
    }

    public virtual void Die(GameObject killer)
    {
        if (_isDead) { return; }

        if(TryGetComponent(out Bot bot))
        {
            bot.StopAllCoroutines();
            bot.OnDeath();
        }

        Instantiate(deathFX, transform.position, Quaternion.identity);

        if(TryGetComponent(out Stats stats)) stats.Deaths++;

        ScoreManager.Instance.OnPlayerDied(gameObject);
        SpawnManager.Instance.Respawn(transform.root.gameObject);

        gameObject.SetActive(false);
        _isDead = true;
    }

    public void Respawn()
    {
        _isDead = false;
        gameObject.SetActive(true);
        GetComponentInChildren<Gun>().RefillMag();

        health = maxHealth;
        if (healthBar != null) healthBar.value = health;

        if (TryGetComponent(out Bot bot))
        {
            bot.OnRespawn();
        }
    }

    WaitForSeconds _delay;
    IEnumerator Heal(float delay)
    {
        yield return _delay ??= new WaitForSeconds(delay);

        while (health < maxHealth)
        {
            health = Mathf.Clamp(health + healthPerSec * Time.deltaTime, 0, maxHealth);
            if(healthBar != null) healthBar.value = health;

            yield return null;
        }
    }
}
