using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Auto-targets the enemy closest to the mouse cursor and fires projectiles.
/// Switches between Angel (Holy Spear) and Devil (Hellfire Bolt) weapons.
/// </summary>
public class WeaponSystem : MonoBehaviour
{
    [Header("Angel Weapon – Holy Spear")]
    public GameObject spearPrefab;
    public float spearFireRate = 0.45f;
    public float spearDamage = 12f;
    public float spearSpeed = 14f;

    [Header("Devil Weapon – Hellfire Bolt")]
    public GameObject hellfirePrefab;
    public float hellfireFireRate = 0.18f;
    public float hellfireDamage = 9f;
    public float hellfireSpeed = 20f;

    [Header("Targeting")]
    public float targetRange = 18f;

    private float fireTimer;
    private DualitySystem dualitySystem;

    void Awake()
    {
        dualitySystem = GetComponent<DualitySystem>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            Enemy target = FindTargetNearMouse();
            if (target != null)
            {
                Shoot(target);

                bool isDevil = dualitySystem.currentForm == PlayerForm.Devil;
                float baseRate = isDevil ? hellfireFireRate : spearFireRate;
                fireTimer = baseRate / dualitySystem.AttackSpeedMultiplier;
            }
        }
    }

    /// <summary>Find the alive enemy closest to the mouse cursor within range.</summary>
    Enemy FindTargetNearMouse()
    {
        var mouse = Mouse.current;
        if (mouse == null) return null;

        Vector2 mouseScreen = mouse.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, 0f));
        mouseWorld.z = 0f;

        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy closest = null;
        float closestDist = float.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null || enemy.isDead) continue;

            float distToPlayer = Vector2.Distance(enemy.transform.position, transform.position);
            if (distToPlayer > targetRange) continue;

            float distToMouse = Vector2.Distance(enemy.transform.position, mouseWorld);
            if (distToMouse < closestDist)
            {
                closestDist = distToMouse;
                closest = enemy;
            }
        }

        return closest;
    }

    void Shoot(Enemy target)
    {
        bool isDevil = dualitySystem.currentForm == PlayerForm.Devil;

        GameObject prefab = isDevil ? hellfirePrefab : spearPrefab;
        float damage = isDevil ? hellfireDamage : spearDamage;
        float speed = isDevil ? hellfireSpeed : spearSpeed;

        if (prefab == null) return;

        Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject proj = Instantiate(prefab, transform.position, Quaternion.Euler(0, 0, angle));
        Projectile projectile = proj.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(direction, speed, damage * dualitySystem.DamageMultiplier);
        }
    }
}
