﻿using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [SerializeField] public GameObject interactText;
    [SerializeField] private Texture2D customAimCursor;
    [SerializeField] private Texture2D customCursor;
    [SerializeField] private int previousMenu;
    
    [Header("Level complete UI objects")]
    [SerializeField] private TMP_Text levelRecordText;
    [SerializeField] private TMP_Text newRecordText;
    [SerializeField] private TMP_Text completedLevelTitle;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private GameObject levelSelectorPauseReturn;

    [SerializeField] private GameObject[] optionTabs;
    [SerializeField] private Button[] optionButtons;

    private UnityEngine.InputSystem.PlayerInput _playerInput;
    private static Guid _playerDeathGuid;
    private static Guid _playerSucceedsGuid;

    // TODO: REMOVE AT LAUNCHES, ONLY USED NOW FOR EASIER CONTROL SETTINGS
    private void Awake()
    {
        // Loads gamedata from file
        GameLauncher.LoadSettings();
        GetComponent<SoundOptions>().LoadSoundSettings();
        GetComponent<GameOptions>().LoadGameSettings();
    }

    private void Start()
    {
        EventSystem.Current.RegisterListener<WinningEvent>(OnPlayerSucceedsLevel, ref _playerSucceedsGuid);
        EventSystem.Current.RegisterListener<PlayerDeathEvent>(OnPlayerDeath, ref _playerDeathGuid);
        _playerInput = FindObjectOfType<UnityEngine.InputSystem.PlayerInput>();

         if (FindObjectOfType<LevelSettings>().IsTutorialLevel() && GameController.TutorialIsOn)
        {
            _playerInput.SwitchCurrentActionMap("MenuControls");
        }
        else
        {
            _playerInput.SwitchCurrentActionMap("PlayerControls");
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.SetCursor(customCursor, new Vector2(customAimCursor.width / 2, customAimCursor.height / 2), CursorMode.Auto);
            return;
        }

        Physics.gravity = new Vector3(0, -Constants.GRAVITY, 0);
        
        if (customAimCursor != null)
        {
            SetAimCursor();
        }
    }

    void SetAimCursor()
    {
        Cursor.SetCursor(customAimCursor, new Vector2(customAimCursor.width / 2, customAimCursor.height / 2), CursorMode.Auto);
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
        if (context.started)
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
        if (context.started)
        {
            if (gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                if (menus[0].gameObject.activeSelf)
                {
                    Unpause();
                } else if (menus[1].gameObject.activeSelf)
                {
                    OpenPauseScreenFromLevelSelector();
                }
            }
        }
    }

    public void OpenPauseScreenFromLevelSelector()
    {
        menus[1].gameObject.SetActive(false);
        menus[0].gameObject.SetActive(true);
        levelSelectorPauseReturn.SetActive(true);
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

        ToggleActionMap(false);
        SetAimCursor();
        GameController.UnpauseGame();
    }

    public void Pause(int index)
    {
        gameObject.transform.parent.GetComponent<AudioSource>().mute = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(true);

        levelSelectorPauseReturn.SetActive(false);
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
        levelSelectorPauseReturn.SetActive(false);
        
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1 && index == 1)
        {
            if (nextLevelButton.IsInteractable())
            {
                nextLevelButton.interactable = false;
            }
        }

        ToggleActionMap(true);
        Cursor.SetCursor(customCursor, new Vector2(customAimCursor.width / 2, customAimCursor.height / 2), CursorMode.Auto);
        GameController.PauseGame();
    }

    private void OnPlayerDeath(PlayerDeathEvent playerDeathEvent)
    {
        StartCoroutine(RestartWhenDead(playerDeathEvent));
    }

    private IEnumerator RestartWhenDead(PlayerDeathEvent playerDeathEvent)
    {
        GameController.SetInputLockState(true);
        GameController.playerIsDead = true;
        yield return new WaitForSecondsRealtime(playerDeathEvent.DeathTime);
        GameController.SetInputLockState(false);
        GameController.playerIsDead = false;
        Restart();
    }

    public void OnPlayerSucceedsLevel(WinningEvent winningEvent)
    {
        LevelCompletionTracker.AddUnlockedLevel(SceneManager.GetActiveScene().buildIndex);
        LevelCompletionTracker.AddUnlockedLevel(SceneManager.GetActiveScene().buildIndex + 1);
        LevelCompletionTracker.SetLevelBest(SceneManager.GetActiveScene().buildIndex,
            FindObjectOfType<LevelTimer>().GetTimePassed());
        
        float bestTime = LevelCompletionTracker.levelRecords[SceneManager.GetActiveScene().buildIndex];
        float minutes = Mathf.FloorToInt(bestTime / 60);
        float seconds = Mathf.FloorToInt(bestTime % 60);
        float milliSeconds = Mathf.Floor(bestTime % 1 * 100);
        
        // If a new record has been set by the player
        if (LevelCompletionTracker.IsTimeNewRecord(SceneManager.GetActiveScene().buildIndex, FindObjectOfType<LevelTimer>().GetTimePassed()))
        {
            newRecordText.color = Color.red;
            newRecordText.text = $"New record: {minutes:00}:{seconds:00}:{milliSeconds:00}";
        }
        else
        {
            newRecordText.color = Color.white;
            newRecordText.text = $"Your time: {minutes:00}:{seconds:00}:{milliSeconds:00}";
        }
        
        levelRecordText.text = $"Best time: {minutes:00}:{seconds:00}:{milliSeconds:00}";
        completedLevelTitle.text = GetComponent<LevelSelector>().levelContainers[SceneManager.GetActiveScene().buildIndex-1].levelName;
        GameLauncher.SaveSettings();
        Pause(1);
    }

    public void LoadScene(int scene)
    {
        if (scene == 0)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            SetAimCursor();
        }

        Unpause();
        LevelCompletionTracker.AddUnlockedLevel(scene);
        SceneManager.LoadScene(scene);
    }


    public void Restart()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 1)
        {
            return;
        }

        
        LevelCompletionTracker.AddUnlockedLevel(SceneManager.GetActiveScene().buildIndex);
        LevelCompletionTracker.AddUnlockedLevel(SceneManager.GetActiveScene().buildIndex + 1);
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

        GetComponent<LevelSelector>().LaunchLevelSelection();
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