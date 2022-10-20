﻿using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [SerializeField] public GameObject interactText;
    [SerializeField] private Texture2D customCursor;

    private void Start()
    {
        SetCustomCursor();
    }

    void SetCustomCursor()
    {
        Cursor.SetCursor(customCursor, new Vector2(3, 3), CursorMode.ForceSoftware);
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
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(true);
        if (!menus[index].activeSelf)
        {
            menus[index].SetActive(true);
        }
        
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GameController.PauseGame();
    }

    public void LoadScene(int scene)
    {
        Unpause();
        if (scene == 0)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            SetCustomCursor();
        }
        SceneManager.LoadScene(scene);
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}