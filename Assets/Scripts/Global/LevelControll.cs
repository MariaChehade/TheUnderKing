using UnityEngine;

public class LevelControll : MonoBehaviour
{
    [SerializeField]
    private TimeController timeController;

    [SerializeField]
    private int startNightNumber = 1;

    [SerializeField]
    private float enemyHealthMultiplierPerNight = 1.25f;

    [SerializeField]
    private int additionalEnemiesPerNight = 2;

    [SerializeField]
    private int firstBossNight = 5;

    [SerializeField]
    private int bossNightInterval = 5;

    [SerializeField]
    private int bossCountPerMilestone = 1;

    [SerializeField]
    private int maxEnemiesPerNight = 15;

    public int CurrentNight { get; private set; } = 1;
    public int EnemiesSpawnedThisNight { get; private set; } = 0;

    public float EnemyHealthMultiplier => Mathf.Max(1f, 1f + ((CurrentNight - startNightNumber) * (enemyHealthMultiplierPerNight - 1f)));

    public int AdditionalEnemies => Mathf.Max(0, (CurrentNight - startNightNumber) * additionalEnemiesPerNight);

    public int MaxEnemiesPerNight => Mathf.Max(1, maxEnemiesPerNight + AdditionalEnemies);

    public int BossCountThisNight
    {
        get
        {
            if (CurrentNight < firstBossNight || bossNightInterval <= 0)
            {
                return 0;
            }

            if ((CurrentNight - firstBossNight) % bossNightInterval != 0)
            {
                return 0;
            }

            var milestones = ((CurrentNight - firstBossNight) / bossNightInterval) + 1;
            return Mathf.Max(0, milestones * bossCountPerMilestone);
        }
    }

    public bool ShouldSpawnBoss => BossCountThisNight > 0;

    public void RegisterEnemySpawn()
    {
        EnemiesSpawnedThisNight++;
    }

    private bool wasNight;

    private void Start()
    {
        if (timeController == null)
        {
            timeController = FindFirstObjectByType<TimeController>();
        }

        wasNight = timeController != null && timeController.IsNight;
        CurrentNight = Mathf.Max(1, startNightNumber);
    }

    private void Update()
    {
        if (timeController == null)
        {
            return;
        }

        if (!wasNight && timeController.IsNight)
        {
            CurrentNight++;
            EnemiesSpawnedThisNight = 0;
            Debug.Log($"Night atual: {CurrentNight}");
        }

        wasNight = timeController.IsNight;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Vector3 screenPosition = Vector3.zero;

        Gizmos.color = Color.yellow;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(screenPosition + Vector3.up * 5f, $"Night: {CurrentNight}");
        UnityEditor.Handles.Label(screenPosition + Vector3.up * 4.5f, $"Inimigos: {EnemiesSpawnedThisNight}/{MaxEnemiesPerNight}");
#endif
    }
}
