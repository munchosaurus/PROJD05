using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [SerializeField] public GameObject interactText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menus[0].activeSelf)
            {
                return;
            }
            Pause(0);
        }
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
        GameController.UnpauseGame();
    }

    public void Pause(int index)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(true);
        if (!menus[index].activeSelf)
        {
            menus[index].SetActive(true);            
        }
        GameController.PauseGame();
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
        Unpause();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}