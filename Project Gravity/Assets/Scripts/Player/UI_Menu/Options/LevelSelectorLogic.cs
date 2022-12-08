using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorLogic : MonoBehaviour
{
    public void SelectLevel()
    {
        FindObjectOfType<LevelSelector>().SelectLevel(GetComponent<Button>());
    }
}