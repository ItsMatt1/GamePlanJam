using UnityEngine;

/// <summary>
/// Spawns enemies around the player at increasing rates.
/// Devil form multiplier makes spawns faster + enemies tougher (applied on Enemy.Start).
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] enemyPrefabs;

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
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnDistance;
        Vector2 pos = (Vector2)player.position + offset;

        int index = Random.Range(0, enemyPrefabs.Length);
        Instantiate(enemyPrefabs[index], pos, Quaternion.identity);
    }
}
