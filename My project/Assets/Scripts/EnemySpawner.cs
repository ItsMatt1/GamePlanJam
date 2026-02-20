using UnityEngine;

/// <summary>
/// Spawns enemies around the player at increasing rates.
/// Devil form multiplier makes spawns faster + enemies tougher (applied on Enemy.Start).
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public GameObject prefab;
        [Tooltip("Earliest time (seconds) since round start when this prefab becomes eligible to spawn.")]
        public float earliestSpawnTime = 0f;
        [Tooltip("Relative weight when picking among eligible prefabs.")]
        public float weight = 1f;
    }

    [Header("Prefabs (with timing)")]
    public SpawnEntry[] spawnTable;

    [Header("Spawning")]
    public float baseSpawnInterval = 1.8f;
    public float minSpawnInterval = 0.25f;
    public float spawnDistance = 14f;
    public int maxEnemies = 150;

    [Header("Difficulty")]
    public float difficultyRampTime = 120f; // seconds to reach peak spawn rate

    private float spawnTimer;
    private float elapsedTime;
    private Transform player;
    private DualitySystem dualitySystem;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            dualitySystem = playerObj.GetComponent<DualitySystem>();
        }
    }

    void Update()
    {
        if (player == null) return;

        elapsedTime += Time.deltaTime;

        float progress = Mathf.Clamp01(elapsedTime / difficultyRampTime);
        float interval = Mathf.Lerp(baseSpawnInterval, minSpawnInterval, progress);

        // Devil form makes enemies spawn faster
        if (dualitySystem != null)
            interval /= dualitySystem.EnemySpawnRateMultiplier;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            int count = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
            if (count < maxEnemies)
                SpawnEnemy();

            spawnTimer = interval;
        }
    }

    void SpawnEnemy()
    {
        if (spawnTable == null || spawnTable.Length == 0) return;

        // Calculate total weight of eligible entries
        float totalWeight = 0f;
        for (int i = 0; i < spawnTable.Length; i++)
        {
            var e = spawnTable[i];
            if (e != null && e.prefab != null && elapsedTime >= e.earliestSpawnTime)
                totalWeight += Mathf.Max(0f, e.weight);
        }

        if (totalWeight <= 0f) return; // nothing eligible yet

        // Pick a weighted random entry
        float pick = Random.Range(0f, totalWeight);
        float cursor = 0f;
        GameObject chosen = null;
        for (int i = 0; i < spawnTable.Length; i++)
        {
            var e = spawnTable[i];
            if (e == null || e.prefab == null || elapsedTime < e.earliestSpawnTime) continue;
            cursor += Mathf.Max(0f, e.weight);
            if (pick <= cursor)
            {
                chosen = e.prefab;
                break;
            }
        }

        if (chosen == null) return;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnDistance;
        Vector2 pos = (Vector2)player.position + offset;

        Instantiate(chosen, pos, Quaternion.identity);
    }
}
