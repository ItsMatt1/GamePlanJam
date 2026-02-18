using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHealth = 20f;
    public float moveSpeed = 2.5f;
    public float contactDamage = 8f;
    public float contactCooldown = 0.8f;

    [Header("Drops")]
    public GameObject xpGemPrefab;

    [HideInInspector] public bool isDead = false;

    private float currentHealth;
    private Transform player;
    private float contactTimer;
    private SpriteRenderer spriteRenderer;
    private DualitySystem dualitySystem;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            dualitySystem = playerObj.GetComponent<DualitySystem>();
        }

        // Scale stats based on current form when spawned
        if (dualitySystem != null)
        {
            maxHealth *= dualitySystem.EnemyHealthMultiplier;
            moveSpeed *= dualitySystem.EnemySpeedMultiplier;
            contactDamage *= dualitySystem.EnemyDamageMultiplier;
        }

        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction.x < 0;

        contactTimer -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(FlashHit());

        if (currentHealth <= 0f)
            Die();
    }

    IEnumerator FlashHit()
    {
        if (spriteRenderer != null)
        {
            Color original = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.08f);
            if (!isDead && spriteRenderer != null)
                spriteRenderer.color = original;
        }
    }

    void Die()
    {
        isDead = true;

        if (dualitySystem != null)
            dualitySystem.RegisterKill();

        if (xpGemPrefab != null)
            Instantiate(xpGemPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && contactTimer <= 0f)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
                contactTimer = contactCooldown;
            }
        }
    }
}
