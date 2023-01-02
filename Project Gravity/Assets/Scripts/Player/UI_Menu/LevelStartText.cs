using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class LevelStartText : MonoBehaviour
{
    [SerializeField] TMP_Text levelName;
    [SerializeField] TMP_Text levelRecord;
    [SerializeField] LevelSelector levelSelector;

    private Guid levelStartGuid;

    private void Start()
    {
        levelName.gameObject.SetActive(false);
        levelRecord.gameObject.SetActive(false);
        int buildID = SceneManager.GetActiveScene().buildIndex;
        SetLevelName(buildID);
        SetLevelRecord(buildID);
        GetComponent<Animator>().Play("LevelStartText");
    }

    private void SetLevelName(int buildID)
    {
        levelName.text = levelSelector.levelContainers[buildID + 1].levelName;
    }

    private void SetLevelRecord(int buildID)
    {
        if(LevelCompletionTracker.LevelHasRecord(buildID))
        {
            float bestTime = LevelCompletionTracker.levelRecords[buildID];

            float minutes = Mathf.FloorToInt(bestTime / 60);
            float seconds = Mathf.FloorToInt(bestTime % 60);
            float milliSeconds = Mathf.Floor(bestTime % 1 * 100);

            levelRecord.text = $"Best time: {minutes:00}:{seconds:00}:{milliSeconds:00}";
        }
        else
        {
            levelRecord.text = "Not completed";
        }
    }
}
