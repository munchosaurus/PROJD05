using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    int activeIndex = 0;
    bool canChange= false;

    IEnumerator CountDownToChangeAllowed()
    {
        canChange = false;
        yield return new WaitForSecondsRealtime(1f);
        canChange = true;
    }

    public void BeginTutorial()
    {
        panels[activeIndex].SetActive(true);
        StartCoroutine(CountDownToChangeAllowed());
    }

    public void ChangeActivePanel(InputAction.CallbackContext cbc)
    {
        if (cbc.started && canChange)
        {
            panels[activeIndex++].SetActive(false);

            if (activeIndex < panels.Length)
            {
                panels[activeIndex].SetActive(true);
                StartCoroutine(CountDownToChangeAllowed());
            }
            else
            {
                canChange = false;
                FindObjectOfType<IngameMenu>().Unpause();
            }
        }
    }
}
