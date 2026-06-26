using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controla a tela de compra de arqueiros.
///
/// Setup:
///   1. Crie um GameObject "ArcherShopUI" com UIDocument → ArcherShop.uxml (Sort Order alto, ex: 8).
///   2. Adicione este componente ao mesmo GameObject.
///   3. Atribua a referência ao ArcherSpawner no Inspector.
/// </summary>
public class ArcherShopController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private ArcherSpawner archerSpawner;

    [Header("Configurações")]
    [Tooltip("Custo em diamantes por arqueiro")]
    [SerializeField] private int archerCost = 5;

    [Tooltip("Custo em pedras para comprar o troféu")]
    [SerializeField] private int trophyStoneCost = 50;

    [Tooltip("Custo em diamantes para comprar o troféu")]
    [SerializeField] private int trophyDiamondCost = 20;

    [Tooltip("Tecla para abrir/fechar a loja")]
    [SerializeField] private KeyCode shopKey = KeyCode.E;

    // ── Elementos UI ──────────────────────────────────────────
    private UIDocument    _document;
    private VisualElement _overlay;
    private Label         _diamondLabel;
    private Label         _stoneLabel;
    private Label         _archerCountLabel;
    private Label         _archerCostLabel;
    private Label         _feedbackLabel;
    private Button        _buyButton;
    private Button        _buyTrophyButton;
    private Button        _closeButton;

    private bool _isOpen;

    // ── Ciclo de vida ──────────────────────────────────────────

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        if (_document == null) { Debug.LogError("ArcherShopController: UIDocument não encontrado."); return; }

        var root = _document.rootVisualElement;
        _overlay          = root.Q<VisualElement>("ShopOverlay");
        _diamondLabel     = root.Q<Label>("DiamondBalance");
        _stoneLabel       = root.Q<Label>("StoneBalance");
        _archerCountLabel = root.Q<Label>("ArcherCount");
        _archerCostLabel  = root.Q<Label>("ArcherCost");
        _feedbackLabel    = root.Q<Label>("FeedbackLabel");
        _buyButton        = root.Q<Button>("BuyArcherButton");
        _buyTrophyButton  = root.Q<Button>("BuyTrophyButton");
        _closeButton      = root.Q<Button>("CloseShopButton");

        if (_buyButton       != null) _buyButton.clicked       += OnBuyClicked;
        if (_buyTrophyButton != null) _buyTrophyButton.clicked += OnBuyTrophyClicked;
        if (_closeButton     != null) _closeButton.clicked     += CloseShop;

        if (_archerCostLabel != null)
            _archerCostLabel.text = $"💎 {archerCost}";

        HideShop();
    }

    private void OnEnable()
    {
        ArcherShopTrigger.OnPlayerEnterCastle += ShowHint;
        ArcherShopTrigger.OnPlayerExitCastle  += OnPlayerLeft;
        PlayerWallet.OnDiamondsChanged        += UpdateDiamondLabel;
        PlayerWallet.OnStonesChanged          += UpdateStoneLabel;
    }

    private void OnDisable()
    {
        ArcherShopTrigger.OnPlayerEnterCastle -= ShowHint;
        ArcherShopTrigger.OnPlayerExitCastle  -= OnPlayerLeft;
        PlayerWallet.OnDiamondsChanged        -= UpdateDiamondLabel;
        PlayerWallet.OnStonesChanged          -= UpdateStoneLabel;
    }

    private void Start()
    {
        if (archerSpawner == null)
            archerSpawner = FindFirstObjectByType<ArcherSpawner>();

        RefreshUI();
    }

    private void Update()
    {
        // Só abre se o player estiver dentro do castelo
        if (Input.GetKeyDown(shopKey) && ArcherShopTrigger.IsPlayerInCastle)
        {
            if (_isOpen) CloseShop();
            else         OpenShop();
        }

        // Fecha se o player sair enquanto aberta
        if (_isOpen && !ArcherShopTrigger.IsPlayerInCastle)
            CloseShop();
    }

    // ── Abrir / Fechar ────────────────────────────────────────

    private void ShowHint()
    {
        // A hint de teclado é tratada no próprio HUD ou overlay — aqui apenas atualizamos a UI
        RefreshUI();
    }

    private void OnPlayerLeft()
    {
        if (_isOpen) CloseShop();
    }

    private void OpenShop()
    {
        _isOpen = true;
        RefreshUI();
        if (_overlay != null) _overlay.style.display = DisplayStyle.Flex;
    }

    private void CloseShop()
    {
        _isOpen = false;
        HideShop();
    }

    private void HideShop()
    {
        if (_overlay != null) _overlay.style.display = DisplayStyle.None;
    }

    // ── Lógica de compra ──────────────────────────────────────

    private void OnBuyClicked()
    {
        if (archerSpawner == null)
        {
            SetFeedback("⚠ ArcherSpawner não encontrado!", isError: true);
            return;
        }

        if (archerSpawner.IsFull)
        {
            SetFeedback("Slots de arqueiro lotados!", isError: true);
            return;
        }

        var wallet = PlayerWallet.Instance;
        if (wallet == null || !wallet.TrySpend(archerCost))
        {
            SetFeedback($"Diamantes insuficientes! (custo: {archerCost} 💎)", isError: true);
            return;
        }

        bool spawned = archerSpawner.TrySpawnArcher();
        if (spawned)
            SetFeedback("Arqueiro contratado! 🏹", isError: false);
        else
            SetFeedback("Falha ao invocar arqueiro.", isError: true);

        RefreshUI();
    }

    // ── Atualização da UI ─────────────────────────────────────

    private void RefreshUI()
    {
        var wallet = PlayerWallet.Instance;
        UpdateDiamondLabel(wallet != null ? wallet.Diamonds : 0);
        UpdateStoneLabel(wallet != null ? wallet.Stones : 0);
        UpdateArcherCount();
        UpdateBuyButton();
        UpdateTrophyButton();
    }

    private void UpdateDiamondLabel(int amount)
    {
        if (_diamondLabel != null)
            _diamondLabel.text = $"💎 {amount}";
    }

    private void UpdateStoneLabel(int amount)
    {
        if (_stoneLabel != null)
            _stoneLabel.text = $"🪨 {amount}";
    }

    private void UpdateArcherCount()
    {
        if (_archerCountLabel == null || archerSpawner == null) return;
        _archerCountLabel.text = $"{archerSpawner.ArcherCount} / {archerSpawner.MaxArchers}";
    }

    private void UpdateBuyButton()
    {
        if (_buyButton == null) return;
        bool canBuy = archerSpawner != null && !archerSpawner.IsFull
                      && PlayerWallet.Instance != null
                      && PlayerWallet.Instance.Diamonds >= archerCost;
        _buyButton.SetEnabled(canBuy);
    }

    private void UpdateTrophyButton()
    {
        if (_buyTrophyButton == null) return;
        var wallet = PlayerWallet.Instance;
        bool canBuy = wallet != null
                      && wallet.Stones   >= trophyStoneCost
                      && wallet.Diamonds >= trophyDiamondCost;
        _buyTrophyButton.SetEnabled(canBuy);
    }

    private void SetFeedback(string message, bool isError)
    {
        if (_feedbackLabel == null) return;
        _feedbackLabel.text = message;
        _feedbackLabel.EnableInClassList("feedback--error",   isError);
        _feedbackLabel.EnableInClassList("feedback--success", !isError);
    }

    // ── Troféu ────────────────────────────────────────────────

    private void OnBuyTrophyClicked()
    {
        var wallet = PlayerWallet.Instance;
        if (wallet == null || !wallet.TrySpendCombined(trophyStoneCost, trophyDiamondCost))
        {
            SetFeedback($"Recursos insuficientes! (custo: \ud83e\udea8 {trophyStoneCost} + \ud83d\udc8e {trophyDiamondCost})", isError: true);
            return;
        }

        SetFeedback("Trofeu conquistado! Voc\u00ea venceu! \ud83c\udfc6", isError: false);
        RefreshUI();
        Castle.TriggerVictory();
    }
}
