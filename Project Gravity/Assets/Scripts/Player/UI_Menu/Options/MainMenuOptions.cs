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

    private void Awake()
    {
        if (GameObject.Find("MainThemeSpeaker(Clone)") == null)
        {
            FindObjectOfType<LevelSelector>().mainTheme = Instantiate(mainThemeSpeaker).GetComponent<AudioSource>();
        }
        
        StartCoroutine(FindObjectOfType<LevelSelector>().StartFadeToBlack(0, Constants.LEVEL_SWITCH_FADE_DURATION * 2, false));
        CompletionLogger.LoadCountfile();
        GameLauncher.LoadSettings();
        LevelCompletionTracker.AddUnlockedLevel(1);
        GetComponent<SoundOptions>().LoadSoundSettings();
        GetComponent<GameOptions>().LoadGameSettings();
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
        StartCoroutine(FindObjectOfType<LevelSelector>().StartFade());
        StartCoroutine(FindObjectOfType<LevelSelector>().StartFadeToBlack(1, Constants.LEVEL_SWITCH_FADE_DURATION * 2, true));
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

        GetComponent<LevelSelector>().LaunchLevelSelection();
    }
}
