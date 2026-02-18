using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Melee slash for the Devil form katana. Spawned by WeaponSystem.
/// Sweeps an arc in front of the player and damages every enemy inside it once.
/// Attach to a prefab with a SpriteRenderer (slash VFX) – no collider needed,
/// hit detection is done via OverlapCircle + angle check.
/// </summary>
public class KatanaSlash : MonoBehaviour
{
    [Header("Visuals")]
    public float slashLifetime = 0.18f;
    public float sweepSpeed = 900f; // degrees per second for the visual rotation

    private Transform origin;
    private Vector2 direction;
    private float range;
    private float arcDegrees;
    private float damage;
    private bool initialized;

    private HashSet<Enemy> alreadyHit = new HashSet<Enemy>();
    private float timer;
    private float currentSweepAngle;
    private float startAngle;

    public void Initialize(Transform attackOrigin, Vector2 dir, float rng, float arc, float dmg)
    {
        origin = attackOrigin;
        direction = dir;
        range = rng;
        arcDegrees = arc;
        damage = dmg;
        initialized = true;

        startAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        currentSweepAngle = startAngle - arcDegrees * 0.5f;

        // First hit check immediately
        DamageEnemiesInArc();
    }

    void Update()
    {
        if (!initialized) return;

        timer += Time.deltaTime;

        // Animate the slash rotation across the arc
        float sweepProgress = timer / slashLifetime;
        currentSweepAngle = (startAngle - arcDegrees * 0.5f) +
                            arcDegrees * Mathf.Clamp01(sweepProgress);
        transform.rotation = Quaternion.Euler(0, 0, currentSweepAngle);

        // Follow the player
        if (origin != null)
        {
            transform.position = origin.position +
                (Vector3)(direction * range * 0.4f);
        }

        // Keep checking for enemies throughout the slash
        DamageEnemiesInArc();

        if (timer >= slashLifetime)
            Destroy(gameObject);
    }

    void DamageEnemiesInArc()
    {
        if (origin == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin.position, range);

        foreach (Collider2D col in hits)
        {
            if (!col.CompareTag("Enemy")) continue;

            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null || enemy.isDead) continue;
            if (alreadyHit.Contains(enemy)) continue;

            // Check if enemy is within the slash arc
            Vector2 toEnemy = ((Vector2)enemy.transform.position - (Vector2)origin.position).normalized;
            float angleToEnemy = Mathf.Atan2(toEnemy.y, toEnemy.x) * Mathf.Rad2Deg;
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(startAngle, angleToEnemy));

            if (angleDiff <= arcDegrees * 0.5f)
            {
                enemy.TakeDamage(damage);
                alreadyHit.Add(enemy);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Editor visualization of the slash arc
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
