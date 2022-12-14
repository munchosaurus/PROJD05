using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditScreen : MonoBehaviour
{
    [Header("Music player audio clips")]
    [SerializeField] private AudioClip mainThemeClip;
    [SerializeField] private AudioClip inGameThemeClip;
    [SerializeField] private AudioClip creditThemeClip;
    [SerializeField] private float bottomvolume;
    [SerializeField] private GameObject black;
    [SerializeField] private AudioSource mainTheme;
    [SerializeField] private GameObject mainThemeSpeaker;

    [SerializeField] private TMP_Text titleText;
    // Update is called once per frame
    private void Start()
    {
        if (GameController.previousSceneIndex != 0)
        {
            titleText.text = "Congratulations";
        }
        
        if (GameObject.Find("MainThemeSpeaker(Clone)") == null)
        {
            mainTheme = Instantiate(mainThemeSpeaker).GetComponent<AudioSource>();
        }

        StartCoroutine(StartFadeToBlack(0, Constants.LEVEL_SWITCH_FADE_DURATION, false));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(StartFade());
        StartCoroutine(StartFadeToBlack(0, Constants.LEVEL_SWITCH_FADE_DURATION * 2, true));
    }
    
    public IEnumerator StartFade()
    {
        float currentTime = 0;
        float start = mainTheme.volume;
        while (currentTime < Constants.LEVEL_SWITCH_FADE_DURATION)
        {
            currentTime += Time.unscaledDeltaTime;
            mainTheme.volume =
                Mathf.Lerp(start, bottomvolume, currentTime / (Constants.LEVEL_SWITCH_FADE_DURATION));
            yield return null;
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            mainTheme.clip = inGameThemeClip;
        } else if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            mainTheme.clip = mainThemeClip;
        } else if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2)
        {
            mainTheme.clip = creditThemeClip;
        }
        
        
        mainTheme.Play();
        currentTime = 0;
        while (currentTime < Constants.LEVEL_SWITCH_FADE_DURATION)
        {
            currentTime += Time.unscaledDeltaTime;
            mainTheme.volume =
                Mathf.Lerp(bottomvolume, start, currentTime / (Constants.LEVEL_SWITCH_FADE_DURATION));
            yield return null;
        }

        GameLauncher.WriteSettings();
    }
    
    public IEnumerator StartFadeToBlack(int sceneToLoad, float duration, bool turnBlack)
    {
        var b = black.GetComponent<Image>();
        var color = b.color;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float val;
            if (turnBlack)
            {
                val = Mathf.Lerp(0, 1, currentTime / duration);
            }
            else
            {
                val = Mathf.Lerp(1, 0, currentTime / duration);
            }

            Color c = new Color(color.r, color.g, color.b, val);
            b.color = c;
            yield return null;
        }
        
        if (turnBlack)
        {
            GameController.previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
            GameLauncher.WriteSettings();
            SceneManager.LoadScene(sceneToLoad);
        }
    }

}
