using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [SerializeField] public GameObject interactText;
    [SerializeField] private Texture2D customCursor;
    [SerializeField] private int previousMenu;
    private static Guid _playerDeathGuid;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }
        
        EventSystem.Current.RegisterListener<PlayerDeathEvent>(OnPlayerDeath, ref _playerDeathGuid);
        
        if (customCursor != null) 
        {
            SetCustomCursor();
        }
    }

    void SetCustomCursor()
    {
        Cursor.SetCursor(customCursor, new Vector2(customCursor.width/2, customCursor.height/2), CursorMode.Auto);
    }

    public void ChangePauseState()
    {
        if (menus[0].activeSelf)
        {
            return;
        }

        Pause(0);
    }

    public void Unpause()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].activeSelf)
            {
                menus[i].SetActive(false);
            }
        }

        if (gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        
        SetCustomCursor();
        GameController.UnpauseGame();
    }

    public void Pause(int index)
    {
        gameObject.transform.parent.GetComponent<AudioSource>().mute = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(true);
        if (!menus[index].activeSelf)
        {
            menus[index].SetActive(true);
        }

        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1 && index == 1)
        {
            if (menus[index].transform.GetChild(0).GetComponent<Button>().IsInteractable())
            {
                menus[index].transform.GetChild(0).GetComponent<Button>().interactable = false;
            }
            else
            {
                menus[index].transform.GetChild(0).GetComponent<Button>().interactable = true;
            }
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GameController.PauseGame();
    }

    private void OnPlayerDeath(PlayerDeathEvent playerDeathEvent)
    {
        StartCoroutine(OpenMenuAfterPlayerDeath(playerDeathEvent));
    }

    private IEnumerator OpenMenuAfterPlayerDeath(PlayerDeathEvent playerDeathEvent)
    {
        GameController.PauseGame();
        yield return new WaitForSecondsRealtime(playerDeathEvent.DeathTime);
        Pause(2);
    }

    public void LoadScene(int scene)
    {
        if (scene == 0)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            SetCustomCursor();
        }
        Unpause();
        LevelCompletionTracker.AddCompletedLevel(scene);
        SceneManager.LoadScene(scene);
    }
    
    

    public void Restart()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings-1)
        {
            return;
        }
        LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void LoadPreviousScene()
    {
        if (SceneManager.GetActiveScene().buildIndex <= 1)
        {
            return;
        }
        LoadScene(SceneManager.GetActiveScene().buildIndex-1);
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

        for (int i = 0; i < menus[3].GetComponentsInChildren<Button>().Length - 1; i++)
        {
            if (i >= SceneManager.sceneCountInBuildSettings - 1)
            {
                menus[3].transform.GetChild(i).GetComponent<Button>().interactable = false;
                continue;
            }

            if (LevelCompletionTracker.unlockedLevels.Count < 1)
            {
                LevelCompletionTracker.AddCompletedLevel(1);
            }
            
            if (!FindObjectOfType<LevelSettings>().GetLevelsAreUnlocked())
            {   if (!LevelCompletionTracker.unlockedLevels.Contains(i+1))
                    {
                        menus[3].transform.GetChild(i).GetComponent<Button>().interactable = false;
                    }
            }
        }
    }

    public void CloseLevelSelector()
    {
        if (menus[3].activeSelf)
        {
            menus[3].SetActive(false);
        }
        Pause(previousMenu);
    }
}