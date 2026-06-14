using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject bossPrefab;

    [SerializeField]
    private TimeController timeController;

    [SerializeField]
    private LevelControll levelControll;

    [SerializeField]
    private float minSpawnSeconds = 1f;

    [SerializeField]
    private float maxSpawnSeconds = 3f;

    private float spawnTimer;
    private int lastProcessedNight = -1;

    private void Start()
    {
        if (levelControll == null)
        {
            levelControll = FindFirstObjectByType<LevelControll>();
        }

        ResetSpawnTimer();
    }

    private void Update()
    {
        if (enemyPrefab == null || timeController == null)
        {
            return;
        }

        if (levelControll == null)
        {
            levelControll = FindFirstObjectByType<LevelControll>();
        }

        if (!timeController.IsNight)
        {
            return;
        }

        TrySpawnBossForCurrentNight();

        if (levelControll != null && levelControll.EnemiesSpawnedThisNight >= levelControll.MaxEnemiesPerNight)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            ResetSpawnTimer();
        }
    }

    private void TrySpawnBossForCurrentNight()
    {
        if (levelControll == null || bossPrefab == null)
        {
            return;
        }

        if (lastProcessedNight == levelControll.CurrentNight)
        {
            return;
        }

        lastProcessedNight = levelControll.CurrentNight;

        var bossCount = levelControll.BossCountThisNight;
        for (var i = 0; i < bossCount; i++)
        {
            SpawnBoss();
        }
    }

    private void SpawnEnemy()
    {
        var instance = Instantiate(enemyPrefab, transform.position, transform.rotation);
        var enemy = instance.GetComponent<BaseEnemy>();

        if (enemy != null && levelControll != null)
        {
            enemy.ApplyHealthMultiplier(levelControll.EnemyHealthMultiplier);
            levelControll.RegisterEnemySpawn();
        }
    }

    private void SpawnBoss()
    {
        var bossInstance = Instantiate(bossPrefab, transform.position, transform.rotation);
        var boss = bossInstance.GetComponent<BaseEnemy>();

        if (boss != null && levelControll != null)
        {
            boss.ApplyHealthMultiplier(levelControll.EnemyHealthMultiplier);
            levelControll.RegisterEnemySpawn();
        }
    }

    private void ResetSpawnTimer()
    {
        if (minSpawnSeconds < 0f)
        {
            minSpawnSeconds = 0f;
        }

        if (maxSpawnSeconds < minSpawnSeconds)
        {
            maxSpawnSeconds = minSpawnSeconds;
        }

        spawnTimer = Random.Range(minSpawnSeconds, maxSpawnSeconds);
    }
}
