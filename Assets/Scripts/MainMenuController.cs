using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private UIDocument _document;
    private Button _playButton;
    private Button _optionsButton;
    private Button _quitButton;

    [Tooltip("Nome exato da cena de jogo que será carregada ao clicar em PLAY")]
    [SerializeField] private string gameSceneName = "Main";

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        if (_document == null)
        {
            Debug.LogError("MainMenuController: nenhum UIDocument encontrado no mesmo GameObject.");
            return;
        }

        var root = _document.rootVisualElement;

        _playButton    = root.Q<Button>("PlayButton");
        _optionsButton = root.Q<Button>("OptionsButton");
        _quitButton    = root.Q<Button>("QuitButton");

        if (_playButton    != null) _playButton.clicked    += OnPlayClicked;
        if (_optionsButton != null) _optionsButton.clicked += OnOptionsClicked;
        if (_quitButton    != null) _quitButton.clicked    += OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnOptionsClicked()
    {
        Debug.Log("Options: implemente seu menu de opções aqui.");
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        if (_playButton    != null) _playButton.clicked    -= OnPlayClicked;
        if (_optionsButton != null) _optionsButton.clicked -= OnOptionsClicked;
        if (_quitButton    != null) _quitButton.clicked    -= OnQuitClicked;
    }
}
