using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSpeaker : MonoBehaviour
{
    public float volume;
    public AudioSource mainMenuSource;

    void Awake()
    {
        mainMenuSource.volume = volume;
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeMusic());
    }

    public IEnumerator FadeMusic()
    {
        float currentTime = 0;
        float start = mainMenuSource.volume;
        while (currentTime < (Constants.LEVEL_SWITCH_FADE_DURATION))
        {
            currentTime += Time.unscaledDeltaTime;
            mainMenuSource.volume =
                Mathf.Lerp(start, volume, currentTime / (Constants.LEVEL_SWITCH_FADE_DURATION));
            yield return null;
        }
    }
}