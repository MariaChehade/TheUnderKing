using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Atualiza o HUD de jogo com o tempo até a próxima fase
/// e a vida do castelo.
///
/// Setup: crie um GameObject com UIDocument (HUD.uxml)
/// e adicione este componente. Atribua as referências no Inspector.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Referências de jogo")]
    [SerializeField] private TimeController timeController;
    [SerializeField] private Castle castle;
    [SerializeField] private LevelControll levelControll;

    // ── Elementos do HUD ──────────────────────────────────────
    private Label _nightLabel;

    private Label _phaseLabel;
    private Label _timeLabel;
    private VisualElement _timeBar;

    private Label _castleHPLabel;
    private VisualElement _castleBar;

    private UIDocument _document;

    // ── Cores / classes por estado ────────────────────────────
    private const string CLASS_DAY   = "phase-day";
    private const string CLASS_NIGHT = "phase-night";

    private const string CLASS_BAR_TIME_DAY   = "bar-fill--time";
    private const string CLASS_BAR_TIME_NIGHT = "bar-fill--time--night";

    private const string CLASS_HP_HIGH   = "bar-fill--health";
    private const string CLASS_HP_MED    = "bar-fill--health--medium";
    private const string CLASS_HP_LOW    = "bar-fill--health--low";

    private const string CLASS_NIGHT_NORMAL = "night-label";
    private const string CLASS_NIGHT_BOSS   = "night-label--boss";

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("HUDController: UIDocument não encontrado.");
            return;
        }

        var root = _document.rootVisualElement;

        _nightLabel    = root.Q<Label>("NightLabel");

        _phaseLabel   = root.Q<Label>("PhaseLabel");
        _timeLabel    = root.Q<Label>("TimeLabel");
        _timeBar      = root.Q<VisualElement>("TimeBar");

        _castleHPLabel = root.Q<Label>("CastleHPLabel");
        _castleBar     = root.Q<VisualElement>("CastleBar");
    }

    private void Start()
    {
        // Tenta encontrar automaticamente se não foram atribuídos no Inspector
        if (timeController == null)
            timeController = FindFirstObjectByType<TimeController>();

        if (castle == null)
            castle = FindFirstObjectByType<Castle>();

        if (levelControll == null)
            levelControll = FindFirstObjectByType<LevelControll>();
    }

    private void Update()
    {
        UpdateNightPanel();
        UpdateTimePanel();
        UpdateCastlePanel();
    }

    // ── Noite ─────────────────────────────────────────────────

    private void UpdateNightPanel()
    {
        if (_nightLabel == null || levelControll == null) return;

        int night = levelControll.CurrentNight;
        bool isBossNight = (night % 5 == 0);

        _nightLabel.text = $"NOITE  {night}";
        _nightLabel.EnableInClassList(CLASS_NIGHT_BOSS,   isBossNight);
        _nightLabel.EnableInClassList(CLASS_NIGHT_NORMAL, !isBossNight);
    }

    // ── Tempo ─────────────────────────────────────────────────

    private void UpdateTimePanel()
    {
        if (timeController == null) return;

        bool isNight = timeController.IsNight;
        float remaining = Mathf.Max(0f, timeController.TimeRemaining);
        float duration  = Mathf.Max(1f, timeController.PhaseDuration);
        float ratio     = remaining / duration;

        // Texto da fase
        if (_phaseLabel != null)
        {
            _phaseLabel.text = isNight ? "🌙 NOITE" : "☀ DIA";
            _phaseLabel.EnableInClassList(CLASS_DAY,   !isNight);
            _phaseLabel.EnableInClassList(CLASS_NIGHT,  isNight);
        }

        // Contagem regressiva  m:ss
        if (_timeLabel != null)
        {
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            _timeLabel.text = $"{minutes}:{seconds:D2}";
        }

        // Barra de tempo (esvazia com o tempo)
        if (_timeBar != null)
        {
            _timeBar.style.width = Length.Percent(ratio * 100f);
            _timeBar.EnableInClassList(CLASS_BAR_TIME_DAY,   !isNight);
            _timeBar.EnableInClassList(CLASS_BAR_TIME_NIGHT,  isNight);
        }
    }

    // ── Castelo ───────────────────────────────────────────────

    private void UpdateCastlePanel()
    {
        if (castle == null) return;

        int current = castle.CurrentHealth;
        int max     = Mathf.Max(1, castle.MaxHealth);
        float ratio = (float)current / max;

        // Texto numérico
        if (_castleHPLabel != null)
            _castleHPLabel.text = $"{current} / {max}";

        // Barra de vida com cor dinâmica
        if (_castleBar != null)
        {
            _castleBar.style.width = Length.Percent(ratio * 100f);

            bool isHigh = ratio > 0.5f;
            bool isMed  = ratio > 0.25f && !isHigh;
            bool isLow  = !isHigh && !isMed;

            _castleBar.EnableInClassList(CLASS_HP_HIGH, isHigh);
            _castleBar.EnableInClassList(CLASS_HP_MED,  isMed);
            _castleBar.EnableInClassList(CLASS_HP_LOW,  isLow);
        }
    }
}
