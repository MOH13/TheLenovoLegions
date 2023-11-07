using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu2 : MonoBehaviour
{
    private UIDocument _document;
    private Button _playButton;
    private Button _exitButton;
    private Button _levelsButton;
    private Button _settingsButton;
    
    [SerializeField] private VisualTreeAsset _levelsButtonsTemplate;
    private VisualElement _levelsButtons;
    private VisualElement _buttonsWrapper;

    void Awake()
    {
        _document = GetComponent<UIDocument>();
        _playButton = _document.rootVisualElement.Q<Button>("PlayButton");
        _playButton.clicked += OnPlayButton;

        _exitButton = _document.rootVisualElement.Q<Button>("ExitButton");
        _exitButton.clicked += OnExitButton;

        _settingsButton = _document.rootVisualElement.Q<Button>("SettingsButton");

        _buttonsWrapper = _document.rootVisualElement.Q<VisualElement>("Buttons");
        _levelsButton = _document.rootVisualElement.Q<Button>("LevelsButton");
        _levelsButton.clicked += OnLevelsButton;

        _levelsButtons = _levelsButtonsTemplate.CloneTree();
        var backButton = _levelsButtons.Q<Button>("BackButton");
        backButton.clicked += OnBackButton;
        var level1Button = _levelsButtons.Q<Button>("Level1Button");
        level1Button.clicked += OnLevel1Button;
        var level2Button = _levelsButtons.Q<Button>("Level2Button");
        level2Button.clicked += OnLevel2Button;
        var level3Button = _levelsButtons.Q<Button>("Level3Button");
        level3Button.clicked += OnLevel3Button;


    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnLevelsButton()
    {
        _buttonsWrapper.Clear();
        _buttonsWrapper.Add(_levelsButtons);

    }

    public void OnLevelButton(int level)
    {
        SceneManager.LoadScene("Level" + level);
    }

    public void OnLevel1Button()
    {
        OnLevelButton(1);
    }

    public void OnLevel2Button()
    {
        OnLevelButton(2);
    }

    public void OnLevel3Button()
    {
        OnLevelButton(3);
    }

    public void OnBackButton()
    {
        _buttonsWrapper.Clear();
        _buttonsWrapper.Add(_playButton);
        _buttonsWrapper.Add(_levelsButton);
        _buttonsWrapper.Add(_settingsButton);
        _buttonsWrapper.Add(_exitButton);
    }


}
