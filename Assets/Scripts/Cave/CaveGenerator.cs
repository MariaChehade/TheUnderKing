using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    [Header("Mapa")]
    public int width = 20;
    public int height = 15;

    [Header("Posição Inicial")]
    public Vector2 startPosition;

    [Header("Prefabs")]
    public GameObject stonePrefab;
    public GameObject diamondPrefab;

    private BlockType[,] grid;

    private void Start()
    {
        GenerateCave();
    }

    private void GenerateCave()
    {
        grid = new BlockType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BlockType type;

                // Camada 1 (0 - 4) somente pedra
                if (y < 5)
                {
                    type = BlockType.Stone;
                }

                // Camada 2 (5 - 29) 5% diamante
                else if (y < 30)
                {
                    if (Random.value < 0.05f)
                    {
                        type = BlockType.Diamond;
                    }
                    else
                    {
                        type = BlockType.Stone;
                    }
                }

                // Camada 3 (30 - 45) 10% diamante
                else
                {
                    if (Random.value < 0.10f)
                    {
                        type = BlockType.Diamond;
                    }
                    else
                    {
                        type = BlockType.Stone;
                    }
                }

                grid[x, y] = type;

                SpawnBlock(type, x, y);
            }
        }
    }

    private void SpawnBlock(BlockType type, int x, int y)
    {
        GameObject prefab = null;

        switch (type)
        {
            case BlockType.Stone:
                prefab = stonePrefab;
                break;

            case BlockType.Diamond:
                prefab = diamondPrefab;
                break;
        }

        Debug.Log($"Tipo: {type}");
        Debug.Log($"Prefab: {prefab}");

        if (prefab == null)
        {
            Debug.LogError($"Prefab NULL para {type}");
            return;
        }

        GameObject obj = Instantiate(
            prefab,
            new Vector3(
                startPosition.x + x,
                startPosition.y - y,
                0
            ),
            Quaternion.identity
        );

        Debug.Log($"Instanciado: {obj.name}");
    }
}