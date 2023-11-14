using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{

    private UIDocument _document;
    private Button _pauseButton;
    private Button _resumeButton;
    private Button _restartButton;
    private Button _mainMenuButton;

    [SerializeField] private VisualTreeAsset _pauseMenuTemplate;
    private VisualElement _pauseMenu;
    private VisualElement _hudLayover;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _pauseButton = _document.rootVisualElement.Q<Button>("PauseButton");
        _pauseButton.clicked += OnPauseButton;

        _hudLayover = _document.rootVisualElement.Q<VisualElement>("HUDLayover");

        _pauseMenu = _pauseMenuTemplate.CloneTree();
        _resumeButton = _pauseMenu.Q<Button>("ResumeButton");
        _resumeButton.clicked += OnResumeButton;
        _restartButton = _pauseMenu.Q<Button>("RestartButton");
        _restartButton.clicked += OnRestartButton;
        _mainMenuButton = _pauseMenu.Q<Button>("MainMenuButton");
        _mainMenuButton.clicked += OnMainMenuButton;

    }

    public void OnPauseButton()
    {
        Time.timeScale = 0;
        _hudLayover.Clear();
        _hudLayover.Add(_pauseMenu);
    }

    public void OnResumeButton()
    {
        _hudLayover.Clear();
        _hudLayover.Add(_pauseButton);
        Time.timeScale = 1;
    }

    public void OnRestartButton()
    {
        // TODO
        var _currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(_currentScene.name);
        Time.timeScale = 1;
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }


    // pausebutton should set time scale to 0, clear the HUD and put in the pauseMenu

    // resumeButton should set time scale to 1, clear the HUD and put the pause button

    // restartButton should load the level again - how do we know which level we are in?

    // Main Menu should load main menu - maybe it should ask if that is what we really want 
}
