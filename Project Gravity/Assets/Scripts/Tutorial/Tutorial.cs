using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    int activeIndex = 0;
    bool done = false;

    private void Awake()
    {
        GameController.PauseGame();
        panels[activeIndex].SetActive(true);
    }

    public void ChangeActivePanel(InputAction.CallbackContext cbc)
    {
        if (cbc.canceled)
        {
            if (!done)
            {
                panels[activeIndex++].SetActive(false);

                if (activeIndex < panels.Length)
                {
                    panels[activeIndex].SetActive(true);
                }
                else
                {
                    done = true;
                    GameController.UnpauseGame();
                }
            }
        }
    }
}
