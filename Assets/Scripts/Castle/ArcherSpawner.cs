using UnityEngine;

/// <summary>
/// Gerencia os slots de arqueiro no castelo.
///
/// Setup no Inspector:
///   - archerPrefab  → prefab do arqueiro
///   - spawnPoints   → array de Transforms (filhos deste GO) que indicam onde cada arqueiro fica
///   - maxArchers    → limite máximo (padrão 4)
/// </summary>
public class ArcherSpawner : MonoBehaviour
{
    [Header("Arqueiro")]
    [SerializeField] private GameObject archerPrefab;

    [Tooltip("Posições onde os arqueiros serão instanciados (slots)")]
    [SerializeField] private Transform[] spawnPoints;

    [Tooltip("Número máximo de arqueiros permitidos")]
    [SerializeField] private int maxArchers = 4;

    // Rastreia quais slots estão ocupados
    private GameObject[] _spawnedArchers;

    public int ArcherCount  { get; private set; }
    public int MaxArchers   => maxArchers;
    public bool IsFull      => ArcherCount >= maxArchers;

    private void Awake()
    {
        int slots = spawnPoints != null ? spawnPoints.Length : 0;
        int size  = Mathf.Max(slots, maxArchers);
        _spawnedArchers = new GameObject[size];
    }

    /// <summary>
    /// Tenta instanciar um arqueiro no próximo slot livre.
    /// Retorna true se bem-sucedido, false se lotado ou sem prefab.
    /// </summary>
    public bool TrySpawnArcher()
    {
        if (archerPrefab == null)
        {
            Debug.LogWarning("ArcherSpawner: archerPrefab não atribuído.");
            return false;
        }

        if (IsFull)
        {
            Debug.Log("ArcherSpawner: slots cheios.");
            return false;
        }

        // Encontra primeiro slot sem arqueiro
        for (int i = 0; i < _spawnedArchers.Length && i < maxArchers; i++)
        {
            if (_spawnedArchers[i] != null) continue;

            Vector3 pos      = GetSpawnPosition(i);
            Quaternion rot   = spawnPoints != null && i < spawnPoints.Length
                                   ? spawnPoints[i].rotation
                                   : Quaternion.identity;
            Transform parent = spawnPoints != null && i < spawnPoints.Length
                                   ? spawnPoints[i]
                                   : transform;

            var archer = Instantiate(archerPrefab, pos, rot, parent);
            _spawnedArchers[i] = archer;
            ArcherCount++;
            return true;
        }

        return false;
    }

    private Vector3 GetSpawnPosition(int index)
    {
        if (spawnPoints != null && index < spawnPoints.Length && spawnPoints[index] != null)
            return spawnPoints[index].position;

        // Fallback: distribui ao longo do spawner
        return transform.position + Vector3.right * (index * 1.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (var sp in spawnPoints)
        {
            if (sp != null)
                Gizmos.DrawWireSphere(sp.position, 0.3f);
        }
    }
}
