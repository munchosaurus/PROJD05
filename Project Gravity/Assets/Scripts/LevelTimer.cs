using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private LevelSettings _levelSettings;
    [SerializeField] private IngameMenu _ingameMenu;
    [SerializeField] private bool timePressure;
    [SerializeField] private float levelTimer;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        _levelSettings = (LevelSettings) FindObjectOfType (typeof(LevelSettings));
        _ingameMenu = (IngameMenu) FindObjectOfType (typeof(IngameMenu));
        if (_levelSettings == null)
        {
            Debug.Log("Cannot find levelsettings in scene, are you sure you have added?");
            return;
        }

        timePressure = _levelSettings.GetLevelIsTimed();
        
        if (!timePressure)
        {
            levelTimer = 0;
        }
        else
        {
            levelTimer = _levelSettings.GetLevelTimeLimit();
        }
    }

    void FixedUpdate()
    {
        if (_levelSettings == null)
        {
            Debug.Log("Cannot find levelsettings in scene, are you sure you have added?");
            return;
        }
        if (timePressure)
        {
            if (levelTimer > 0)
            {
                levelTimer -= Time.deltaTime;
            }
            else
            {
                if (_ingameMenu != null && !GameController.GetPlayerInputIsLocked())
                {
                    _ingameMenu.Pause(2);
                }
            }
        }
        else
        {
            levelTimer += Time.deltaTime;
        }
        
        DisplayTime(levelTimer);
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = timeToDisplay % 1 * 100;
        
        text.text = $"{minutes:00}:{seconds:00}:{milliSeconds:00}";
        
    }
}
