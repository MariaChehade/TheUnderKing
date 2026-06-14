using UnityEngine;

public class Castle : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || currentHealth <= 0)
        {
            return;
        }

        Debug.Log("Castle took damage: " + amount);
        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (currentHealth == 0)
        {
            Time.timeScale = 0f;
            Debug.Log("GameOver");
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Vector3 castlePosition = transform.position;

        // Exibir vida do castelo acima do castelo
        Vector3 healthTextPosition = castlePosition + Vector3.up * 3f;

        // Desenhar um retângulo representando a barra de vida
        float healthPercentage = (float)currentHealth / maxHealth;
        float barWidth = 2f;
        float barHeight = 0.3f;

        Vector3 barMin = healthTextPosition + Vector3.left * (barWidth * 0.5f);
        Vector3 barMax = healthTextPosition + Vector3.right * (barWidth * 0.5f);

        // Barra de fundo (vermelha)
        Gizmos.color = Color.red;
        DrawGizmoRectangle(barMin, barMax, barHeight);

        // Barra de vida (verde)
        Gizmos.color = Color.green;
        Vector3 healthBarMax = barMin + Vector3.right * (barWidth * healthPercentage);
        DrawGizmoRectangle(barMin, healthBarMax, barHeight);

        // Texto com a vida
        Gizmos.color = Color.white;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(healthTextPosition + Vector3.up * 0.5f, $"HP: {currentHealth}/{maxHealth}");
#endif
    }

    private void DrawGizmoRectangle(Vector3 min, Vector3 max, float height)
    {
        Vector3 topLeft = min + Vector3.up * (height * 0.5f);
        Vector3 topRight = max + Vector3.up * (height * 0.5f);
        Vector3 bottomLeft = min - Vector3.up * (height * 0.5f);
        Vector3 bottomRight = max - Vector3.up * (height * 0.5f);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}

