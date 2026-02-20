using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Angel Passive Regen")]
    public float angelRegenRate = 2f;

    [Header("Invincibility After Hit")]
    public float iFrameDuration = 0.6f;

    [Header("Events")]
    public UnityEvent<float> onHealthChanged; // 0-1 normalized
    public UnityEvent onPlayerDeath;

    private float iFrameTimer;
    private bool isDead;
    private DualitySystem dualitySystem;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        currentHealth = maxHealth;
        dualitySystem = GetComponent<DualitySystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return;

        // Angel form passive regeneration
        if (dualitySystem != null && dualitySystem.currentForm == PlayerForm.Angel)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth = Mathf.Min(currentHealth + angelRegenRate * Time.deltaTime, maxHealth);
                onHealthChanged?.Invoke(currentHealth / maxHealth);
            }
        }

        // Invincibility blink
        if (iFrameTimer > 0f)
        {
            iFrameTimer -= Time.deltaTime;
            if (spriteRenderer != null)
                spriteRenderer.enabled = Mathf.PingPong(Time.time * 12f, 1f) > 0.5f;
        }
        else if (spriteRenderer != null && !spriteRenderer.enabled)
        {
            spriteRenderer.enabled = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || iFrameTimer > 0f) return;

        currentHealth -= damage;
        iFrameTimer = iFrameDuration;
        onHealthChanged?.Invoke(Mathf.Max(0f, currentHealth) / maxHealth);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            isDead = true;
            spriteRenderer.enabled = true;
            onPlayerDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }
}
