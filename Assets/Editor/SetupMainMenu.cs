using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupMainMenu : EditorWindow
{
    [MenuItem("Tools/Setup Main Menu Scene")]
    public static void RunSetup()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject uiObj = GameObject.Find("UI_MainMenu");
        if (uiObj == null)
        {
            uiObj = new GameObject("UI_MainMenu");
        }

        UIDocument uiDoc = uiObj.GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            uiDoc = uiObj.AddComponent<UIDocument>();
        }

        VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/MainMenu.uxml");
        if (uxml == null)
        {
            Debug.LogError("Could not find MainMenu.uxml at Assets/UI/MainMenu.uxml");
            return;
        }
        uiDoc.visualTreeAsset = uxml;

        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI/MainMenuPanelSettings.asset");
        if (panelSettings == null)
        {
            panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            AssetDatabase.CreateAsset(panelSettings, "Assets/UI/MainMenuPanelSettings.asset");
            Debug.Log("Created new PanelSettings at Assets/UI/MainMenuPanelSettings.asset");
        }
        uiDoc.panelSettings = panelSettings;

        if (uiObj.GetComponent<MainMenuController>() == null)
        {
            uiObj.AddComponent<MainMenuController>();
        }

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.backgroundColor = new Color(0.81f, 0.27f, 0.46f);
            mainCam.clearFlags = CameraClearFlags.SolidColor;
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("Main Menu Scene setup complete!");
    }
}
