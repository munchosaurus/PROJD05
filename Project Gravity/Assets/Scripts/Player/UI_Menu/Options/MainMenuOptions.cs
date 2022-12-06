using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuOptions : MonoBehaviour
{
    [SerializeField] private GameObject optionsObject;
    [SerializeField] private GameObject levelObject;
    [SerializeField] private GameObject[] optionTabs;

    private void Awake()
    {
        CompletionLogger.LoadCountfile();
        // Loads gamedata from file
        GameLauncher.LoadSettings();
        LevelCompletionTracker.AddUnlockedLevel(1);
        GetComponent<SoundOptions>().LoadSoundSettings();
        GetComponent<GameOptions>().LoadGameSettings();
    }

    public void OpenOptionsMenu()
    {
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
        GameLauncher.WriteSettings();
        SceneManager.LoadScene(1);
    }
    
    public void CloseLevelSelector()
    {
        if (levelObject.activeSelf)
        {
            levelObject.SetActive(false);
        }
    }
    
    public void OpenLevelSelector()
    {
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
