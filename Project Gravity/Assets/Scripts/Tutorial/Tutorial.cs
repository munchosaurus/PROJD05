using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    [SerializeField] public GameObject[] panels;
    [SerializeField] private AudioSource mainTheme;
    private IngameMenu _ingameMenu;
    public int activeIndex = 0;
    public bool canChange = false;

    private void Awake()
    {
        _ingameMenu = FindObjectOfType<IngameMenu>();
    }

    IEnumerator CountDownToChangeAllowed()
    {
        canChange = false;
        yield return new WaitForSecondsRealtime(1f);
        canChange = true;
    }

    public void BeginTutorial()
    {
        try
        {
            mainTheme = GameObject.Find("MainThemeSpeaker(Clone)").GetComponent<AudioSource>();
            mainTheme.volume *= 0.5f;
        }
        catch (Exception e)
        {
            Debug.Log("No main theme speaker found" + e);
        }

        panels[activeIndex].SetActive(true);
        StartCoroutine(CountDownToChangeAllowed());
    }

    public void OpenPanel()
    {
        panels[activeIndex].SetActive(true);
        StartCoroutine(CountDownToChangeAllowed());
    }

    public void ChangeActivePanel(InputAction.CallbackContext cbc)
    {
        if (cbc.started && canChange && !_ingameMenu.transform.GetChild(0).gameObject.activeSelf)
        {
            panels[activeIndex++].SetActive(false);

            if (activeIndex < panels.Length)
            {
                OpenPanel();
            }
            else
            {
                canChange = false;
                try
                {
                    mainTheme.volume *= 2f;
                }
                catch (Exception e)
                {
                    Debug.Log("No main theme speaker found" + e);
                }

                Time.timeScale = 1;
                //FindObjectOfType<IngameMenu>().Unpause();
            }
        }
    }
}
