using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Angel form: auto-shoots Holy Spears at the enemy closest to the mouse.
/// Devil form: swings a Katana slash arc toward the enemy closest to the mouse.
/// </summary>
public class WeaponSystem : MonoBehaviour
{
    [Header("Angel Weapon – Holy Spear (ranged)")]
    public GameObject spearPrefab;
    public float spearFireRate = 0.45f;
    public float spearDamage = 12f;
    public float spearSpeed = 14f;
    public AudioClip spearSound;

    [Header("Devil Weapon – Katana (melee)")]
    public GameObject katanaSlashPrefab;
    public float katanaSwingRate = 0.22f;
    public float katanaDamage = 18f;
    public float katanaRange = 2.5f;
    public float katanaArc = 120f; // sweep angle in degrees
    public AudioClip katanaSound;

    [Header("Targeting")]
    public float targetRange = 18f;

    private float fireTimer;
    private DualitySystem dualitySystem;
    private AudioSource audioSource;

    void Awake()
    {
        dualitySystem = GetComponent<DualitySystem>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            bool isDevil = dualitySystem.currentForm == PlayerForm.Devil;

            if (isDevil)
            {
                // Katana swings toward mouse direction even without a target
                SwingKatana();
                fireTimer = katanaSwingRate / dualitySystem.AttackSpeedMultiplier;
            }
            else
            {
                Enemy target = FindTargetNearMouse();
                if (target != null)
                {
                    ShootSpear(target);
                    fireTimer = spearFireRate / dualitySystem.AttackSpeedMultiplier;
                }
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

    Vector2 GetMouseWorldDirection()
    {
        var mouse = Mouse.current;
        if (mouse == null) return Vector2.right;

        Vector2 mouseScreen = mouse.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, 0f));

        Vector2 dir = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
        return dir == Vector2.zero ? Vector2.right : dir;
    }

    void ShootSpear(Enemy target)
    {
        if (spearPrefab == null) return;

        Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject proj = Instantiate(spearPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        Projectile projectile = proj.GetComponent<Projectile>();

        if (projectile != null)
            projectile.Initialize(direction, spearSpeed, spearDamage * dualitySystem.DamageMultiplier);

        if (audioSource != null && spearSound != null)
            audioSource.PlayOneShot(spearSound);
    }

    void SwingKatana()
    {
        if (katanaSlashPrefab == null) return;

        Vector2 swingDir = GetMouseWorldDirection();
        float angle = Mathf.Atan2(swingDir.y, swingDir.x) * Mathf.Rad2Deg;

        // Spawn the slash slightly in front of the player
        Vector3 spawnPos = transform.position + (Vector3)(swingDir * katanaRange * 0.4f);
        GameObject slash = Instantiate(katanaSlashPrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        KatanaSlash slashScript = slash.GetComponent<KatanaSlash>();
        if (slashScript != null)
        {
            float finalDamage = katanaDamage * dualitySystem.DamageMultiplier;
            slashScript.Initialize(transform, swingDir, katanaRange, katanaArc, finalDamage);
        }

        if (audioSource != null && katanaSound != null)
            audioSource.PlayOneShot(katanaSound);
    }
}
