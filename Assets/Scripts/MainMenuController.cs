using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    private UIDocument _document;
    private Button _playButton;
    private Button _optionsButton;
    private Button _quitButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        
        if (_document == null)
        {
            Debug.LogError("MainMenuController requires a UIDocument component on the same GameObject.");
            return;
        }

        var root = _document.rootVisualElement;
        
        _playButton = root.Q<Button>("PlayButton");
        _optionsButton = root.Q<Button>("OptionsButton");
        _quitButton = root.Q<Button>("QuitButton");

        if (_playButton != null) _playButton.clicked += OnPlayClicked;
        if (_optionsButton != null) _optionsButton.clicked += OnOptionsClicked;
        if (_quitButton != null) _quitButton.clicked += OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        Debug.Log("Play button clicked! Load your game scene here.");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    private void OnOptionsClicked()
    {
        Debug.Log("Options button clicked! Show options menu here.");
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit button clicked! Exiting game.");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        if (_playButton != null) _playButton.clicked -= OnPlayClicked;
        if (_optionsButton != null) _optionsButton.clicked -= OnOptionsClicked;
        if (_quitButton != null) _quitButton.clicked -= OnQuitClicked;
    }
}
