using UnityEngine;
using UnityEngine.Events;

public enum PlayerForm
{
    Angel,
    Devil
}

/// <summary>
/// Core duality mechanic. Kills in Angel form build corruption.
/// When corruption is full, the player transforms into Devil form.
/// In Devil form corruption drains over time; kills slow the drain.
/// When corruption empties, the player reverts to Angel.
/// </summary>
public class DualitySystem : MonoBehaviour
{
    [Header("Corruption")]
    public float maxCorruption = 100f;
    public float corruptionPerKill = 8f;
    public float purificationRate = 4f; // corruption drain per second in devil form

    [Header("Devil Form Modifiers")]
    public float devilAttackSpeedMultiplier = 2.0f;
    public float devilDamageMultiplier = 1.5f;
    public float devilMoveSpeedMultiplier = 1.15f;
    public float devilEnemyHealthMultiplier = 1.6f;
    public float devilEnemySpeedMultiplier = 1.4f;
    public float devilEnemySpawnRateMultiplier = 1.6f;
    public float devilEnemyDamageMultiplier = 1.5f;

    [Header("Runtime State (read-only)")]
    public PlayerForm currentForm = PlayerForm.Angel;
    public float corruption = 0f;
    public int totalKills = 0;

    [Header("Events")]
    public UnityEvent onTransformToDevil;
    public UnityEvent onTransformToAngel;
    public UnityEvent<float> onCorruptionChanged; // 0-1 normalized

    private SpriteRenderer spriteRenderer;

    // Accessors for other systems
    public float AttackSpeedMultiplier =>
        currentForm == PlayerForm.Devil ? devilAttackSpeedMultiplier : 1f;

    public float DamageMultiplier =>
        currentForm == PlayerForm.Devil ? devilDamageMultiplier : 1f;

    public float EnemyHealthMultiplier =>
        currentForm == PlayerForm.Devil ? devilEnemyHealthMultiplier : 1f;

    public float EnemySpeedMultiplier =>
        currentForm == PlayerForm.Devil ? devilEnemySpeedMultiplier : 1f;

    public float EnemySpawnRateMultiplier =>
        currentForm == PlayerForm.Devil ? devilEnemySpawnRateMultiplier : 1f;

    public float EnemyDamageMultiplier =>
        currentForm == PlayerForm.Devil ? devilEnemyDamageMultiplier : 1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (currentForm == PlayerForm.Devil)
        {
            corruption -= purificationRate * Time.deltaTime;
            onCorruptionChanged?.Invoke(corruption / maxCorruption);

            if (corruption <= 0f)
            {
                TransformToAngel();
            }
        }
    }

    /// <summary>Call this whenever the player kills an enemy.</summary>
    public void RegisterKill()
    {
        totalKills++;

        if (currentForm == PlayerForm.Angel)
        {
            corruption += corruptionPerKill;
            onCorruptionChanged?.Invoke(corruption / maxCorruption);

            if (corruption >= maxCorruption)
                TransformToDevil();
        }
        else
        {
            // Kills in devil form slow the revert by adding a fraction back
            corruption = Mathf.Min(corruption + corruptionPerKill * 0.25f, maxCorruption);
            onCorruptionChanged?.Invoke(corruption / maxCorruption);
        }
    }

    void TransformToDevil()
    {
        currentForm = PlayerForm.Devil;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.85f, 0.15f, 0.15f); // red tint

        onTransformToDevil?.Invoke();
    }

    void TransformToAngel()
    {
        currentForm = PlayerForm.Angel;
        corruption = 0f;
        onCorruptionChanged?.Invoke(0f);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        onTransformToAngel?.Invoke();
    }
}
