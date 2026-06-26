using System;
using UnityEngine;

/// <summary>
/// Singleton que rastreia as moedas do jogador (💎 Diamantes e 🪨 Pedras).
///
/// Setup: crie um GameObject "PlayerWallet" na cena e adicione este componente.
/// Subscreve automaticamente ao Block.OnBlockBroken para contar ambos os recursos.
/// </summary>
public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance { get; private set; }

    /// <summary>Disparado sempre que o saldo de diamantes muda.</summary>
    public static event Action<int> OnDiamondsChanged;

    /// <summary>Disparado sempre que o saldo de pedras muda.</summary>
    public static event Action<int> OnStonesChanged;

    [Header("Saldo inicial")]
    [SerializeField] private int startingDiamonds = 0;
    [SerializeField] private int startingStones   = 0;

    private int _diamonds;
    private int _stones;

    public int Diamonds => _diamonds;
    public int Stones   => _stones;

    // ── Ciclo de vida ──────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _diamonds = startingDiamonds;
        _stones   = startingStones;
    }

    private void OnEnable()  => Block.OnBlockBroken += HandleBlockBroken;
    private void OnDisable() => Block.OnBlockBroken -= HandleBlockBroken;

    // ── Lógica ────────────────────────────────────────────────

    private void HandleBlockBroken(BlockType type)
    {
        if (type == BlockType.Diamond)
        {
            _diamonds++;
            OnDiamondsChanged?.Invoke(_diamonds);
        }
        else if (type != BlockType.Empty)
        {
            _stones++;
            OnStonesChanged?.Invoke(_stones);
        }
    }

    /// <summary>
    /// Tenta gastar <paramref name="amount"/> diamantes.
    /// Retorna true e desconta o saldo se houver fundos suficientes.
    /// </summary>
    public bool TrySpend(int amount)
    {
        if (amount <= 0 || _diamonds < amount) return false;
        _diamonds -= amount;
        OnDiamondsChanged?.Invoke(_diamonds);
        return true;
    }

    /// <summary>
    /// Tenta gastar <paramref name="stones"/> pedras.
    /// </summary>
    public bool TrySpendStones(int stones)
    {
        if (stones <= 0 || _stones < stones) return false;
        _stones -= stones;
        OnStonesChanged?.Invoke(_stones);
        return true;
    }

    /// <summary>
    /// Tenta gastar pedras E diamantes ao mesmo tempo (transação atômica).
    /// Só desconta se tiver ambos os recursos.
    /// </summary>
    public bool TrySpendCombined(int stones, int diamonds)
    {
        if (_stones < stones || _diamonds < diamonds) return false;
        _stones   -= stones;
        _diamonds -= diamonds;
        OnStonesChanged?.Invoke(_stones);
        OnDiamondsChanged?.Invoke(_diamonds);
        return true;
    }

    /// <summary>Adiciona diamantes ao saldo (usado por debug / drops futuros).</summary>
    public void AddDiamonds(int amount)
    {
        if (amount <= 0) return;
        _diamonds += amount;
        OnDiamondsChanged?.Invoke(_diamonds);
    }

    /// <summary>Adiciona pedras ao saldo.</summary>
    public void AddStones(int amount)
    {
        if (amount <= 0) return;
        _stones += amount;
        OnStonesChanged?.Invoke(_stones);
    }
}
