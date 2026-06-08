using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private TimeController timeController;

    [SerializeField]
    private float minSpawnSeconds = 1f;

    [SerializeField]
    private float maxSpawnSeconds = 3f;

    [SerializeField]
    private int maxEnemiesOnScreen = 10;

    private float spawnTimer;

    private void Start()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        if (enemyPrefab == null || timeController == null || !timeController.IsNight)
        {
            return;
        }

        if (maxEnemiesOnScreen > 0 && CountEnemiesOnScreen() >= maxEnemiesOnScreen)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            Debug.Log("Inimigo spawnado");

            Instantiate(enemyPrefab, transform.position, transform.rotation);

            ResetSpawnTimer();
        }
    }

    private static int CountEnemiesOnScreen()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
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