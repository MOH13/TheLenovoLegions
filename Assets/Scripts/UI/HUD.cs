using LL.Input;
using System;
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
    private Button _hubButton;
    private Button _mainMenuButton;
    private Button _inventoryButton;
    
    [SerializeField]
    private GameObject _inventory;

    [SerializeField] private VisualTreeAsset _pauseMenuTemplate;
    private VisualElement _pauseMenu;
    private VisualElement _hudLayover;
    private VisualElement _hudButtons;

    private MyPlayerInput input;


    private void OnEnable()
    {
        if (input == null)
            input = new MyPlayerInput();
        input.UI.Enable();
    }

    private void Start()
    {
        _document = GetComponent<UIDocument>();
        _pauseButton = _document.rootVisualElement.Q<Button>("PauseButton");
        _pauseButton.clicked += OnPauseButton;

        _hudLayover = _document.rootVisualElement.Q<VisualElement>("HUDLayover");
        _hudButtons = _document.rootVisualElement.Q<VisualElement>("Buttons");

        _pauseMenu = _pauseMenuTemplate.CloneTree();
        _pauseMenu.style.height = Length.Percent(100);
        _resumeButton = _pauseMenu.Q<Button>("ResumeButton");
        _resumeButton.clicked += OnResumeButton;
        _restartButton = _pauseMenu.Q<Button>("RestartButton");
        _restartButton.clicked += OnRestartButton;
        _hubButton = _pauseMenu.Q<Button>("HubButton");
        _hubButton.clicked += OnHubButton;
        _mainMenuButton = _pauseMenu.Q<Button>("MainMenuButton");
        _mainMenuButton.clicked += OnMainMenuButton;

        _inventoryButton = _document.rootVisualElement.Q<Button>("InventoryButton");
        _inventoryButton.clicked += OnInventoryButton;
    }

    private void Update()
    {
        if (input.UI.Inventory.WasPressedThisFrame()) {
            if (!InventoryActive && !PauseMenuActive) {
                OnInventoryButton();
            } else if (InventoryActive && !PauseMenuActive)
            {
                OnCloseButton();
            }
        }

        if (input.UI.Pause.WasPressedThisFrame())
        {
            if (!PauseMenuActive && !InventoryActive) {
                OnPauseButton();
            } else if (PauseMenuActive && !InventoryActive)
            {
                OnResumeButton();
            }
        }
    }

    public void OnPauseButton()
    {
        Time.timeScale = 0;
        PauseMenuActive = true;
        _hudButtons.visible = false;
        _hudLayover.Add(_pauseMenu);
    }

    public void OnInventoryButton()
    {        
        InventoryActive = true;
        _inventory.SetActive(true);

    }

    public void OnCloseButton()
    {
        InventoryActive = false;
        _hudButtons.visible = true;
        Time.timeScale = 1;
    }

    public void OnResumeButton()
    {
        PauseMenuActive = false;
        _hudLayover.Remove(_pauseMenu);
        _hudButtons.visible = true;
        Time.timeScale = 1;
    }

    public void OnRestartButton()
    {
        PauseMenuActive = false;
        var _currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(_currentScene.name);
        Time.timeScale = 1;
    }

    public void OnHubButton()
    {
        PauseMenuActive = false;
        SceneManager.LoadScene("HubLevel");
        Time.timeScale = 1;
    }

    public void OnMainMenuButton()
    {
        PauseMenuActive = false;
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    private Boolean PauseMenuActive { get; set; }

    private Boolean InventoryActive { get; set; }


    // pausebutton should set time scale to 0, clear the HUD and put in the pauseMenu

    // resumeButton should set time scale to 1, clear the HUD and put the pause button

    // restartButton should load the level again - how do we know which level we are in?

    // Main Menu should load main menu - maybe it should ask if that is what we really want 
}
