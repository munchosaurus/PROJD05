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
    [SerializeField] private AudioSource mainTheme;
    [SerializeField] private AudioClip mainThemeClip;
    [SerializeField] private float bottomvolume;

    private void Awake()
    {
        mainTheme = GameObject.Find("MainThemeSpeaker").GetComponent<AudioSource>();
        CompletionLogger.LoadCountfile();
        // Loads gamedata from file
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
        StartCoroutine(StartFade());
        StartCoroutine(FindObjectOfType<LevelSelector>().StartFadeToBlack(1, Constants.LEVEL_SWITCH_FADE_DURATION * 2, true));
    }

    public IEnumerator StartFade()
    {
        float currentTime = 0;
        float start = mainTheme.volume;
        while (currentTime < (Constants.LEVEL_SWITCH_FADE_DURATION * 2))
        {
            currentTime += Time.unscaledDeltaTime;
            mainTheme.volume = Mathf.Lerp(start, bottomvolume, currentTime / (Constants.LEVEL_SWITCH_FADE_DURATION * 2));
            yield return null;
        }
        mainTheme.clip = mainThemeClip;
        mainTheme.Play();
        currentTime = 0;
        while (currentTime < (Constants.LEVEL_SWITCH_FADE_DURATION * 2))
        {
            currentTime += Time.unscaledDeltaTime;
            mainTheme.volume = Mathf.Lerp(bottomvolume, start, currentTime / (Constants.LEVEL_SWITCH_FADE_DURATION * 2));
            yield return null;
        }

        GameLauncher.WriteSettings();
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
