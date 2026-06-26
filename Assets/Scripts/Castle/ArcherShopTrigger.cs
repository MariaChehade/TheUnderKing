using System;
using UnityEngine;

/// <summary>
/// Detecta quando o player entra/sai da zona do castelo.
///
/// Setup:
///   1. Adicione este componente a um GameObject filho do castelo.
///   2. O mesmo GameObject deve ter um BoxCollider2D marcado como IsTrigger.
///   3. O player deve ter a tag "Player".
/// </summary>
public class ArcherShopTrigger : MonoBehaviour
{
    public static event Action OnPlayerEnterCastle;
    public static event Action OnPlayerExitCastle;

    public static bool IsPlayerInCastle { get; private set; }

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning("ArcherShopTrigger: Collider2D foi definido como IsTrigger automaticamente.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsPlayerInCastle = true;
        OnPlayerEnterCastle?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsPlayerInCastle = false;
        OnPlayerExitCastle?.Invoke();
    }
}
