using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    [Header("Level Container settings")] 
    public LevelContainer[] levelContainers;
    [SerializeField] private GameObject scrollviewObjectTemplate;
    [SerializeField] private GameObject scrollviewParent;
    [SerializeField] private TMP_Text levelDescription;
    [SerializeField] private TMP_Text levelSelectorRecordText;
    [SerializeField] private Image levelSelectorImage;
    [SerializeField] private Button levelSelectorPlay;
    private Button[] _levelContainerButtons;
    private IngameMenu _ingameMenu;
    private int _selectedLevel;
    
    void Start()
    {
        _ingameMenu = GetComponent<IngameMenu>();
        levelSelectorPlay.onClick.AddListener(OnLevelSelectorPlayPressed);
        SetupLevelContainers();
    }

    public void LaunchLevelSelection()
    {
        SelectLevel(_levelContainerButtons[SceneManager.GetActiveScene().buildIndex - 1]);
        _levelContainerButtons[SceneManager.GetActiveScene().buildIndex - 1].Select();
    }
    
    public void SelectLevel(Button selectedButton)
    {
        int internalLevelIndex = 0;
        for (int i = 0; i < _levelContainerButtons.Length; i++)
        {
            if (selectedButton.GetInstanceID() == _levelContainerButtons[i].GetInstanceID())
            {
                internalLevelIndex = i;
                break;
            }
        }

        int levelID = internalLevelIndex + 1;

        levelDescription.text = levelContainers[internalLevelIndex].levelDescription;
        levelSelectorImage.sprite = levelContainers[internalLevelIndex].levelSprite;
        string text = "Best time: ";
        if (LevelCompletionTracker.LevelHasRecord(levelID))
        {
            float bestTime = LevelCompletionTracker.levelRecords[levelID];

            float minutes = Mathf.FloorToInt(bestTime / 60);
            float seconds = Mathf.FloorToInt(bestTime % 60);
            float milliSeconds = Mathf.Floor(bestTime % 1 * 100);

            text += $"{minutes:00}:{seconds:00}:{milliSeconds:00}";
            levelSelectorPlay.interactable = true;
        }

        if (LevelCompletionTracker.unlockedLevels.Contains(levelID))
        {
            levelSelectorPlay.interactable = true;
        }
        else
        {
            levelSelectorPlay.interactable = false;
        }

        _selectedLevel = levelID;
        levelSelectorRecordText.text = text;
    }
    
    public void SetupLevelContainers()
    {
        _levelContainerButtons = new Button[levelContainers.Length];
        for (int i = 0; i < levelContainers.Length; i++)
        {
            GameObject go = Instantiate(scrollviewObjectTemplate, scrollviewParent.transform, false);
            go.GetComponentInChildren<TMP_Text>().text = levelContainers[i].levelName;
            _levelContainerButtons[i] = go.GetComponent<Button>();
        }
    }

    private void OnLevelSelectorPlayPressed()
    {
        if (LevelCompletionTracker.unlockedLevels.Contains(_selectedLevel))
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                _ingameMenu.LoadScene(_selectedLevel);
            }
            else
            {
                GameLauncher.SaveSettings();
                SceneManager.LoadScene(_selectedLevel);
            }
        }
    }
}
