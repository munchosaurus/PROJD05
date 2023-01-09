using System;
using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private LevelSettings _levelSettings;
    [SerializeField] private IngameMenu _ingameMenu;
    [SerializeField] private bool timePressure;
    [SerializeField] private float levelTimer;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float minutes;
    [SerializeField] private float seconds;
    [SerializeField] private float milliSeconds;
    private static Guid _levelStartsGuid;
    private bool shouldCount;
    private bool _shouldKeepCounting;
    private Guid _playerSucceedsGuid;

    private void Start()
    {
        EventSystem.Current.RegisterListener<WinningEvent>(OnPlayerSucceedsLevel, ref _playerSucceedsGuid);
        EventSystem.Current.RegisterListener<LevelStartEvent>(OnLevelStarts, ref _levelStartsGuid);
        _levelSettings = (LevelSettings) FindObjectOfType(typeof(LevelSettings));
        _ingameMenu = (IngameMenu) FindObjectOfType(typeof(IngameMenu));
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

    public void OnLevelStarts(LevelStartEvent levelStartEvent)
    {
        _shouldKeepCounting = true;
    }

    private void OnPlayerSucceedsLevel(WinningEvent winningEvent)
    {
        _shouldKeepCounting = false;
    }

    void FixedUpdate()
    {
        if (!GameController.IsPaused() && _shouldKeepCounting)
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
        }

        DisplayTime(levelTimer);
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        minutes = Mathf.FloorToInt(timeToDisplay / 60);
        seconds = Mathf.FloorToInt(timeToDisplay % 60);
        milliSeconds = Mathf.Floor(timeToDisplay % 1 * 100);
        text.text = $"{minutes:00}:{seconds:00}:{milliSeconds:00}";
    }

    public float GetTimePassed()
    {
        return levelTimer;
    }
}