using UnityEngine;

/// <summary>
/// Generic projectile. Works for both Holy Spear (piercing) and Hellfire Bolt.
/// Attach to a prefab that has Rigidbody2D (Kinematic) and a trigger Collider2D.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Behaviour")]
    public float lifetime = 5f;
    public bool piercing = false;
    public int maxPierceCount = 3;

    private Vector2 direction;
    private float speed;
    private float damage;
    private int pierceCount = 0;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 dir, float spd, float dmg)
    {
        direction = dir;
        speed = spd;
        damage = dmg;

        if (rb != null)
            rb.linearVelocity = direction * speed;

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.isDead)
        {
            enemy.TakeDamage(damage);
        }

        if (piercing)
        {
            pierceCount++;
            if (pierceCount >= maxPierceCount)
                Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
