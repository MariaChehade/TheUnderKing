using System;
using UnityEngine;

public class Castle : MonoBehaviour
{
    /// <summary>Disparado quando o castelo chega a 0 de vida.</summary>
    public static event Action OnGameOver;

    /// <summary>Disparado quando o player compra o troféu e vence o jogo.</summary>
    public static event Action OnVictory;

    /// <summary>Dispara o evento de vitória. Chame a partir de sistemas externos.</summary>
    public static void TriggerVictory()
    {
        OnVictory?.Invoke();
    }

    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

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
            Debug.Log("GameOver");
            OnGameOver?.Invoke();
            Time.timeScale = 0f;
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

