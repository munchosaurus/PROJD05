using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private bool timePressure;
    [SerializeField] private float levelTimer;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        // TODO: ADD CONTAINER FOR ALL LEVELS
        //levelTimer = GetLevelTime(INDEX);
        //timePressure = GetLevelTime(INDEX);

        if (!timePressure)
        {
            levelTimer = 0;
        }
    }

    void Update()
    {
        if (timePressure)
        {
            if (levelTimer > 0)
            {
                levelTimer -= Time.deltaTime;
            }
            else
            {
                //TODO: GAME LOST UI
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
        float milliSeconds = timeToDisplay % 1 * 1000;
        
        text.text = $"{minutes:00}:{seconds:00}:{milliSeconds:000}";
        
    }
}
