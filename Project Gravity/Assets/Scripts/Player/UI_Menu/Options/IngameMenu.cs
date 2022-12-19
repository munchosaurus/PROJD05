using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [SerializeField] public GameObject interactText;
    [SerializeField] private Texture2D customAimCursor;
    [SerializeField] private Texture2D customMenuCursor;
    [SerializeField] private int previousMenu;

    [Header("Level complete UI objects")] [SerializeField]
    private TMP_Text levelRecordText;

    [SerializeField] private TMP_Text newRecordText;
    [SerializeField] private TMP_Text completedLevelTitle;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private GameObject levelCompleteReturn;

    [SerializeField] private GameObject[] optionTabs;
    [SerializeField] private GameObject mainThemeSpeaker;
    [SerializeField] private GameObject playerGui;
    public bool newSceneHasBeenLoaded;
    private PlayerInput _playerInput;
    private LevelSelector _levelSelector;
    private static Guid _playerDeathGuid;
    private static Guid _playerSucceedsGuid;
    private bool _playerWon;

    // TODO: REMOVE AT LAUNCHES, ONLY USED NOW FOR EASIER CONTROL SETTINGS
    private void Awake()
    {
        _levelSelector = FindObjectOfType<LevelSelector>();
        if (GameObject.Find("MainThemeSpeaker(Clone)") == null)
        {
            _levelSelector.mainTheme = Instantiate(mainThemeSpeaker).GetComponent<AudioSource>();
            _levelSelector.mainTheme.clip = _levelSelector.inGameThemeClip;
            _levelSelector.mainTheme.Play();
        }
        
        // Loads gamedata from file
        GameLauncher.LoadSettings();
        CompletionLogger.gravityChanges = 0;
        GetComponent<SoundOptions>().LoadSoundSettings();
        GetComponent<GameOptions>().LoadGameSettings();
        GetComponent<ControlOptions>().SetControlImagesAndTexts();
        StartCoroutine(_levelSelector
            .StartFadeToBlack(0, Constants.LEVEL_SWITCH_FADE_DURATION, false));
    }

    private void Start()
    {
        EventSystem.Current.RegisterListener<WinningEvent>(OnPlayerSucceedsLevel, ref _playerSucceedsGuid);
        EventSystem.Current.RegisterListener<PlayerDeathEvent>(OnPlayerDeath, ref _playerDeathGuid);

        _playerInput = FindObjectOfType<PlayerInput>();

        if (FindObjectOfType<LevelSettings>().IsTutorialLevel() && GameController.TutorialIsOn)
        {
            _playerInput.SwitchCurrentActionMap("MenuControls");
        }
        else
        {
            _playerInput.SwitchCurrentActionMap("PlayerControls");
        }

        Physics.gravity = new Vector3(0, -GravityController.GravityMultiplier * Constants.GRAVITY, 0);

        if (customAimCursor != null)
        {
            SetAimCursor();
        }
    }

    void SetAimCursor()
    {
        Cursor.SetCursor(customAimCursor, new Vector2(customAimCursor.width / 2, customAimCursor.height / 2),
            CursorMode.Auto);
    }

    void SetMenuCursor()
    {
        
        Cursor.SetCursor(customMenuCursor, new Vector2(0, 0),
            CursorMode.Auto);
    }

    public void ToggleActionMap(bool paused)
    {
        if (paused)
        {
            if (_playerInput != null)
            {
                _playerInput.SwitchCurrentActionMap("MenuControls");
            }
        }
        else
        {
            if (_playerInput != null)
            {
                _playerInput.SwitchCurrentActionMap("PlayerControls");
            }
        }
    }

    public void ChangePauseState(InputAction.CallbackContext context)
    {
        if (context.started && !GameController.PlayerIsDead && !newSceneHasBeenLoaded)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Debug.Log("Nice try David");
                return;
            }

            if (menus[0].gameObject.activeSelf)
            {
                Unpause();
                return;
            }

            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].activeSelf)
                {
                    return;
                }
            }

            Pause(0);
        }
    }

    public void ClosePauseScreen(InputAction.CallbackContext context)
    {
        if (context.canceled && !GameController.PlayerIsDead && !newSceneHasBeenLoaded)
        {
            if (gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                if (menus[0].gameObject.activeSelf)
                {
                    if (_playerWon)
                    {
                        ToggleLevelCompleteMenu(false);
                        return;
                    }

                    Unpause();
                }
                else if (menus[1].gameObject.activeSelf)
                {
                    ToggleLevelCompleteMenu(true);
                }
                else if (menus[3].gameObject.activeSelf)
                {
                    OpenPauseScreenFromLevelSelector();
                }
                else if (menus[4].gameObject.activeSelf)
                {
                    CloseOptionsMenu();
                }
            }
        }
    }

    public void ToggleLevelCompleteMenu(bool open)
    {
        if (open)
        {
            menus[1].gameObject.SetActive(false);
            menus[0].gameObject.SetActive(true);
            levelCompleteReturn.SetActive(true);
            return;
        }

        menus[1].gameObject.SetActive(true);
        menus[0].gameObject.SetActive(false);
        levelCompleteReturn.SetActive(false);
    }

    public void OpenPauseScreenFromLevelSelector()
    {
        menus[3].gameObject.SetActive(false);
        menus[0].gameObject.SetActive(true);
    }

    public void Unpause()
    {
        if (menus.Length > 0)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].activeSelf)
                {
                    menus[i].SetActive(false);
                }
            }
        }

        if (gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        playerGui.SetActive(true);
        ToggleActionMap(false);
        SetAimCursor();
        GameController.UnpauseGame();
    }

    public void Pause(int index)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        gameObject.transform.parent.GetComponent<AudioSource>().mute = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(true);
        
        foreach (var menu in menus)
        {
            if (menu.gameObject.activeSelf)
            {
                menu.SetActive(false);
            }
        }

        if (!menus[index].activeSelf)
        {
            menus[index].SetActive(true);
        }

        if (_playerWon)
        {
            levelCompleteReturn.SetActive(true);
        }
        else
        {
            levelCompleteReturn.SetActive(false);
        }

        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2 && index == 1)
        {
            nextLevelButton.GetComponentInChildren<TMP_Text>().text = "Continue";
        }

        playerGui.SetActive(false);
        ToggleActionMap(true);
        SetMenuCursor();
        GameController.PauseGame();
    }

    private void OnPlayerDeath(PlayerDeathEvent playerDeathEvent)
    {
        StartCoroutine(RestartWhenDead(playerDeathEvent));
    }

    private IEnumerator RestartWhenDead(PlayerDeathEvent playerDeathEvent)
    {
        CompletionLogger.lose = 1;
        CompletionLogger.win = 0;
        GameController.SetInputLockState(true);
        GameController.PlayerIsDead = true;
        yield return new WaitForSecondsRealtime(playerDeathEvent.DeathTime);
        Restart();
    }

    public void OnPlayerSucceedsLevel(WinningEvent winningEvent)
    {
        if (!_playerWon)
        {
            _playerWon = true;
            LevelCompletionTracker.AddUnlockedLevel(SceneManager.GetActiveScene().buildIndex);
            LevelCompletionTracker.AddUnlockedLevel(SceneManager.GetActiveScene().buildIndex + 1);
            LevelCompletionTracker.SetLevelBest(SceneManager.GetActiveScene().buildIndex,
                FindObjectOfType<LevelTimer>().GetTimePassed());

            float bestTime = LevelCompletionTracker.levelRecords[SceneManager.GetActiveScene().buildIndex];
            float minutes = Mathf.FloorToInt(bestTime / 60);
            float seconds = Mathf.FloorToInt(bestTime % 60);
            float milliSeconds = Mathf.Floor(bestTime % 1 * 100);

            float elapsedTime = FindObjectOfType<LevelTimer>().GetTimePassed();
            float elapsedMinutes = Mathf.FloorToInt(elapsedTime / 60);
            float elapsedSeconds = Mathf.FloorToInt(elapsedTime % 60);
            float elapsedMilliseconds = Mathf.Floor(elapsedTime % 1 * 100);

            // If a new record has been set by the player
            if (LevelCompletionTracker.IsTimeNewRecord(SceneManager.GetActiveScene().buildIndex,
                    FindObjectOfType<LevelTimer>().GetTimePassed()))
            {
                newRecordText.color = Color.red;
                newRecordText.text = $"New Record: {minutes:00}:{seconds:00}:{milliSeconds:00}";
            }
            else
            {
                newRecordText.color = Color.white;
                newRecordText.text = $"Your Time: {elapsedMinutes:00}:{elapsedSeconds:00}:{elapsedMilliseconds:00}";
            }

            levelRecordText.text = $"Best Time: {minutes:00}:{seconds:00}:{milliSeconds:00}";
            completedLevelTitle.text = _levelSelector
                .levelContainers[SceneManager.GetActiveScene().buildIndex - 1].levelName;
            GameLauncher.SaveLevels();
            CompletionLogger.lose = 0;
            CompletionLogger.win = 1;
            CompletionLogger.finishTime = elapsedTime;
            CompletionLogger.WriteCompletionLog();
        }
    }

    public void LoadScene(int scene)
    {
        newSceneHasBeenLoaded = true;
        if (scene == 0)
        {
            SetMenuCursor();
        }
        else
        {
            SetAimCursor();
        }

        if (scene == SceneManager.sceneCountInBuildSettings - 1 || scene == 0)
        {
            StartCoroutine(_levelSelector
                .StartFade(scene));
            StartCoroutine(_levelSelector
                .StartFadeToBlack(scene, Constants.LEVEL_SWITCH_FADE_DURATION * 2, true));
        }
        else
        {
            StartCoroutine(_levelSelector
                .StartFadeToBlack(scene, Constants.LEVEL_SWITCH_FADE_DURATION, true));
        }
    }

    public void RestartWithRButton(InputAction.CallbackContext context)
    {
        if (context.started && !GameController.PlayerIsDead && !_playerWon)
        {
            Restart();
        }
    }


    public void Restart()
    {
        if (!_playerWon)
        {
            CompletionLogger.finishTime = FindObjectOfType<LevelTimer>().GetTimePassed();
            CompletionLogger.WriteCompletionLog();
        }

        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2)
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            LevelCompletionTracker.AddUnlockedLevel(SceneManager.GetActiveScene().buildIndex);
            LevelCompletionTracker.AddUnlockedLevel(SceneManager.GetActiveScene().buildIndex + 1);
            LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void LoadPreviousScene()
    {
        if (SceneManager.GetActiveScene().buildIndex <= 1)
        {
            return;
        }

        LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenLevelSelector(int index)
    {
        previousMenu = index;
        for (int i = 0; i < menus.Length - 1; i++)
        {
            if (menus[i].activeSelf)
            {
                menus[i].SetActive(false);
            }
        }

        if (!menus[3].activeSelf)
        {
            menus[3].SetActive(true);
        }

        _levelSelector.LaunchLevelSelection();
    }

    public void OpenOptionsMenu(int index)
    {
        previousMenu = index;
        for (int i = 0; i < menus.Length - 1; i++)
        {
            if (menus[i].activeSelf)
            {
                menus[i].SetActive(false);
            }
        }

        if (!menus[4].activeSelf)
        {
            menus[4].SetActive(true);
        }

        OnOptionTabButtonClick(0);
    }

    public void CloseOptionsMenu()
    {
        if (menus[4].activeSelf)
        {
            menus[4].SetActive(false);
        }

        Pause(previousMenu);
    }

    public void CloseLevelSelector()
    {
        if (menus[3].activeSelf)
        {
            menus[3].SetActive(false);
        }

        Pause(previousMenu);
    }

    public void OnOptionTabButtonClick(int index)
    {
        foreach (var go in optionTabs)
        {
            go.SetActive(false);
        }

        optionTabs[index].SetActive(true);
    }
}