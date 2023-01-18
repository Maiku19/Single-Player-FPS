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

    bool isDead = false;
    Coroutine healing;

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
        if(healing != null) StopCoroutine(healing);
        healing = StartCoroutine(Heal(healingDelay));

        if (health <= 0)
        {
            Die(damager.gameObject);
        }
    }

    public virtual void Die(GameObject killer)
    {
        if (isDead) { return; }

        if(GetComponent<Bot>() != null)
        {
            GetComponent<Bot>().StopAllCoroutines();
            GetComponent<Bot>().OnDeath();
        }

        Instantiate(deathFX, transform.position, Quaternion.identity);
        ScoreManager.Instance.OnPlayerDied(gameObject);
        if(TryGetComponent(out Stats stats)) stats.deaths += 1;
        isDead = true;
        SpawnManager.Instance.Respawn(Mike.MikeTransform.GetParentOfParents(transform).gameObject);
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        isDead = false;
        gameObject.SetActive(true);
        GetComponentInChildren<Gun>().RefillMag();

        health = maxHealth;
        if (healthBar != null) healthBar.value = health;

        if (GetComponent<Bot>() != null)
        {
            GetComponent<Bot>().OnRespawn();
        }
    }

    IEnumerator Heal(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (health < maxHealth)
        {
            health += healthPerSec * Time.deltaTime;
            if(healthBar != null) healthBar.value = health;

            health = Mathf.Clamp(health, 0, maxHealth);

            yield return null;
        }
    }
}
