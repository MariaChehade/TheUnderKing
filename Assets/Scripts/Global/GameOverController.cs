using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Exibe a tela de Game Over quando o castelo chega a 0 de vida.
///
/// Setup:
///   1. Crie um GameObject na cena com UIDocument apontando para GameOver.uxml.
///   2. Adicione este componente ao mesmo GameObject.
///   3. O documento deve começar desativado (ou a tela invisível via display:none),
///      pois o script ativa/exibe ao receber o evento Castle.OnGameOver.
/// </summary>
public class GameOverController : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Nome exato da cena de jogo para reiniciar")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Tooltip("Nome exato da cena do menu principal")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // ── Elementos ─────────────────────────────────────────────
    private VisualElement _overlay;
    private UIDocument    _document;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        if (_document == null)
        {
            Debug.LogError("GameOverController: UIDocument não encontrado.");
            return;
        }

        var root = _document.rootVisualElement;
        _overlay = root.Q<VisualElement>("GameOverOverlay");

        // Começa oculto
        HideScreen();

        // Registra botões
        root.Q<Button>("RestartButton") ?.RegisterCallback<ClickEvent>(_ => Restart());
        root.Q<Button>("MainMenuButton")?.RegisterCallback<ClickEvent>(_ => GoToMainMenu());
        root.Q<Button>("QuitButton")    ?.RegisterCallback<ClickEvent>(_ => QuitGame());
    }

    private void OnEnable()  => Castle.OnGameOver += ShowScreen;
    private void OnDisable() => Castle.OnGameOver -= ShowScreen;

    // ── Visibilidade ──────────────────────────────────────────

    private void ShowScreen()
    {
        if (_overlay == null) return;
        _overlay.style.display = DisplayStyle.Flex;
        // Garante que a UI possa ser interagida mesmo com Time.timeScale = 0
    }

    private void HideScreen()
    {
        if (_overlay == null) return;
        _overlay.style.display = DisplayStyle.None;
    }

    // ── Ações dos botões ──────────────────────────────────────

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
