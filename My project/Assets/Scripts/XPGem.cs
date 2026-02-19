using UnityEngine;

/// <summary>
/// Dropped by enemies on death. Flies toward the player when close,
/// and heals a small amount on collection.
/// </summary>
public class XPGem : MonoBehaviour
{
    [Header("Settings")]
    public float attractRange = 3f;
    public float attractSpeed = 10f;
    public float healAmount = 3f;
    public float pickupRadius = 0.5f;

    private Transform player;
    private bool attracted;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Auto-destroy after a while if not collected
        Destroy(gameObject, 30f);
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < attractRange)
            attracted = true;

        if (attracted)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, player.position, attractSpeed * Time.deltaTime);

            if (dist < pickupRadius)
                Collect();
        }
    }

    void Collect()
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.Heal(healAmount);

        if (GameManager.Instance != null)
            GameManager.Instance.AddGemScore();

        Destroy(gameObject);
    }
}
