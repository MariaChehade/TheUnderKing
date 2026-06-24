using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla a tela de pausa do jogo.
/// Adicione este componente a um GameObject com um UIDocument
/// que use o PauseMenu.uxml como Source Asset.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement _overlay;
    private Button _resumeButton;
    private Button _mainMenuButton;
    private Button _quitButton;

    [Tooltip("Nome da cena do menu principal")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public static bool IsPaused { get; private set; }

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        if (_document == null)
        {
            Debug.LogError("PauseMenuController: nenhum UIDocument encontrado no mesmo GameObject.");
            return;
        }

        var root = _document.rootVisualElement;

        _overlay      = root.Q<VisualElement>("PauseOverlay");
        _resumeButton = root.Q<Button>("ResumeButton");
        _mainMenuButton = root.Q<Button>("MainMenuButton");
        _quitButton   = root.Q<Button>("QuitButton");

        if (_resumeButton   != null) _resumeButton.clicked   += Resume;
        if (_mainMenuButton != null) _mainMenuButton.clicked += GoToMainMenu;
        if (_quitButton     != null) _quitButton.clicked     += QuitGame;

        // Começa oculto
        HideMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    private void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        ShowMenu();
    }

    private void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        HideMenu();
    }

    private void GoToMainMenu()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ShowMenu()
    {
        if (_overlay != null)
            _overlay.style.display = DisplayStyle.Flex;
    }

    private void HideMenu()
    {
        if (_overlay != null)
            _overlay.style.display = DisplayStyle.None;
    }

    private void OnDestroy()
    {
        // Garante que o jogo não fique travado em pausa ao trocar de cena
        if (IsPaused)
        {
            IsPaused = false;
            Time.timeScale = 1f;
        }

        if (_resumeButton   != null) _resumeButton.clicked   -= Resume;
        if (_mainMenuButton != null) _mainMenuButton.clicked -= GoToMainMenu;
        if (_quitButton     != null) _quitButton.clicked     -= QuitGame;
    }
}
