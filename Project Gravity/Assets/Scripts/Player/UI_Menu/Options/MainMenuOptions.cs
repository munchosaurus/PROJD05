using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuOptions : MonoBehaviour
{
    [SerializeField] private GameObject optionsObject;
    [SerializeField] private GameObject levelObject;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject[] optionTabs;
    [SerializeField] private GameObject mainThemeSpeaker;
    [SerializeField] private Texture2D customMenuCursor;
    private LevelSelector _levelSelector;
    

    private void Awake()
    {
        _levelSelector = FindObjectOfType<LevelSelector>();
        if (GameObject.Find("MainThemeSpeaker(Clone)") == null)
        {
            _levelSelector.mainTheme = Instantiate(mainThemeSpeaker).GetComponent<AudioSource>();
        }
        SetMenuCursor();
        StartCoroutine(_levelSelector.StartFadeToBlack(0, Constants.LEVEL_SWITCH_FADE_DURATION * 2, false));
        CompletionLogger.LoadCountfile();
        GameLauncher.LoadSettings();
        LevelCompletionTracker.AddUnlockedLevel(1);
        GetComponent<SoundOptions>().LoadSoundSettings();
        GetComponent<GameOptions>().LoadGameSettings();
    }
    
    void SetMenuCursor()
    {
        Cursor.SetCursor(customMenuCursor, new Vector2(customMenuCursor.width / 4, customMenuCursor.height / 6),
            CursorMode.Auto);
    }

    public void OpenOptionsMenu()
    {
        panel.SetActive(false);
        if (levelObject.activeSelf)
        {
            levelObject.SetActive(false);
        }
        if (!optionsObject.activeSelf)
        {
            optionsObject.SetActive(true);
        }
        OnOptionTabButtonClick(0);
    }

    public void CloseOptionsMenu()
    {
        panel.SetActive(true);
        if (optionsObject.activeSelf)
        {
            optionsObject.SetActive(false);
        }
    }
    
    public void OnOptionTabButtonClick(int index)
    {
        foreach (var go in optionTabs)
        {
            go.SetActive(false);
        }
        
        optionTabs[index].SetActive(true);
    }

    public void StartGame()
    {
        StartCoroutine(_levelSelector.StartFade(1));
        StartCoroutine(_levelSelector.StartFadeToBlack(1, Constants.LEVEL_SWITCH_FADE_DURATION * 2, true));
    }

    public void CloseLevelSelector()
    {
        panel.SetActive(true);
        if (levelObject.activeSelf)
        {
            levelObject.SetActive(false);
        }
    }
    
    public void OpenLevelSelector()
    {
        panel.SetActive(false);
        if (optionsObject.activeSelf)
        {
            optionsObject.SetActive(false);
        }
        if (!levelObject.activeSelf)
        {
            levelObject.SetActive(true);
        }

        _levelSelector.LaunchLevelSelection();
    }

    public void OpenCredits()
    {
        StartCoroutine(_levelSelector.StartFade(SceneManager.sceneCountInBuildSettings - 1));
        StartCoroutine(_levelSelector.StartFadeToBlack(SceneManager.sceneCountInBuildSettings - 1,
            Constants.LEVEL_SWITCH_FADE_DURATION * 2, true));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
