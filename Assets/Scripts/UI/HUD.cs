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
        private Button _inventoryButton;

        private GameObject _inventory;

        [SerializeField] private VisualTreeAsset _pauseMenuTemplate;
        private VisualElement _pauseMenu;
        private VisualElement _hudLayover;
        private VisualElement _hudButtons;

        //[SerializeField] private VisualTreeAsset _inventoryTemplate;
        // private VisualElement _inventory;
        //private Button _closeButton;

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            _pauseButton = _document.rootVisualElement.Q<Button>("PauseButton");
            _pauseButton.clicked += OnPauseButton;
            

            _hudLayover = _document.rootVisualElement.Q<VisualElement>("HUDLayover");
            _hudButtons = _document.rootVisualElement.Q<VisualElement>("Buttons");

            _pauseMenu = _pauseMenuTemplate.CloneTree();
            _resumeButton = _pauseMenu.Q<Button>("ResumeButton");
            _resumeButton.clicked += OnResumeButton;
            _restartButton = _pauseMenu.Q<Button>("RestartButton");
            _restartButton.clicked += OnRestartButton;
            _mainMenuButton = _pauseMenu.Q<Button>("MainMenuButton");
            _mainMenuButton.clicked += OnMainMenuButton;

            //_inventory = _inventoryTemplate.CloneTree();
            //_closeButton = _inventory.Q<Button>("close-button");
            //_closeButton.clicked += OnCloseButton;

            _inventoryButton = _document.rootVisualElement.Q<Button>("InventoryButton");
            _inventoryButton.clicked += OnInventoryButton;

        _inventory = GameObject.Find("Inventory");
    }

        public void OnPauseButton()
        {
            Time.timeScale = 0;
            //_hudLayover.Clear();
            _hudButtons.SetEnabled(false);
        _hudButtons.visible = false;
        _hudLayover.Add(_pauseMenu);
        }

        public void OnInventoryButton()
        {
            _inventory.SetActive(true);

        }

        public void OnCloseButton()
        {
            
            _hudButtons.visible = true;
            _hudButtons.SetEnabled(true);
            Time.timeScale = 1;
        }

        public void OnResumeButton()
        {
            _hudLayover.Remove(_pauseMenu);
            _hudButtons.SetEnabled(true);
            _hudButtons.visible = true;
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