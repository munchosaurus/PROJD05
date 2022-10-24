using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardLogic : MonoBehaviour
{
    [SerializeField] private IngameMenu menu;
    [SerializeField] private string playerTag;
    void Start()
    {
        menu = FindObjectOfType<IngameMenu>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (menu != null)
            {
                menu.Pause(2);
            }
            else
            {
                Debug.Log("Cannot find menu script");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (menu != null)
            {
                menu.Pause(2);
            }
            else
            {
                Debug.Log("Cannot find menu script");
            }
        }
    }
}
