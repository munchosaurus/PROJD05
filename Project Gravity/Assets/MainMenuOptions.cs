using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuOptions : MonoBehaviour
{
    [SerializeField] private GameObject options;

    public void OpenOptionsMenu()
    {
        if (!options.activeSelf)
        {
            options.SetActive(true);
        }
    }

    public void CloseOptionsMenu()
    {
        if (options.activeSelf)
        {
            options.SetActive(false);
        }
    }
}
