using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Exibe a tela de vitória quando Castle.OnVictory é disparado.
///
/// Setup:
///   1. Crie um GameObject "VictoryUI" com UIDocument → Victory.uxml (Sort Order alto, ex: 9).
///   2. Adicione este componente ao mesmo GameObject.
/// </summary>
public class VictoryController : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private string gameSceneName     = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private VisualElement _overlay;
    private UIDocument    _document;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        if (_document == null) { Debug.LogError("VictoryController: UIDocument não encontrado."); return; }

        var root = _document.rootVisualElement;
        _overlay = root.Q<VisualElement>("VictoryOverlay");

        root.Q<Button>("RestartButton") ?.RegisterCallback<ClickEvent>(_ => Restart());
        root.Q<Button>("MainMenuButton")?.RegisterCallback<ClickEvent>(_ => GoToMainMenu());
        root.Q<Button>("QuitButton")    ?.RegisterCallback<ClickEvent>(_ => QuitGame());

        HideScreen();
    }

    private void OnEnable()  => Castle.OnVictory += ShowScreen;
    private void OnDisable() => Castle.OnVictory -= ShowScreen;

    private void ShowScreen()
    {
        if (_overlay == null) return;
        _overlay.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }

    private void HideScreen()
    {
        if (_overlay == null) return;
        _overlay.style.display = DisplayStyle.None;
    }

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
