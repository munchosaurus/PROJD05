using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Data;

public class LevelSelector : MonoBehaviour
{
    [Header("Level Container settings")] public LevelContainer[] levelContainers;
    [SerializeField] private GameObject scrollviewObjectTemplate;
    [SerializeField] private GameObject scrollviewParent;
    [SerializeField] private TMP_Text levelDescription;
    [SerializeField] private TMP_Text levelSelectorRecordText;
    [SerializeField] private Image levelSelectorImage;
    [SerializeField] private Button levelSelectorPlay;
    [SerializeField] private byte disabledLevelTextAlpha;
    [SerializeField] public AudioSource mainTheme;

    [Header("Music player audio clips")] [SerializeField]
    private AudioClip mainThemeClip;

    [SerializeField] public AudioClip inGameThemeClip;
    [SerializeField] private AudioClip creditThemeClip;
    [SerializeField] private float bottomvolume;
    [SerializeField] private RectTransform scrollRectTransform;
    [SerializeField] private GameObject black;
    private Button[] _levelContainerButtons;
    private int _selectedLevel;
    private GameObject lastSelected;
    private RectTransform selectedButton;


    void Start()
    {
        try
        {
            mainTheme = GameObject.FindGameObjectWithTag("MainMenuSpeaker").GetComponent<AudioSource>();
        }
        catch (Exception e)
        {
            Debug.Log("No Main theme in scene" + e);
        }
        
        levelSelectorPlay.onClick.AddListener(OnLevelSelectorPlayPressed);
        SetupLevelContainers();
    }

    public IEnumerator StartFade(int levelToLoad)
    {
        float currentTime = 0;
        float start = mainTheme.volume;
        while (currentTime < (Constants.LEVEL_SWITCH_FADE_DURATION))
        {
            currentTime += Time.unscaledDeltaTime;
            mainTheme.volume =
                Mathf.Lerp(start, bottomvolume, currentTime / (Constants.LEVEL_SWITCH_FADE_DURATION));
            yield return null;
        }

        if (levelToLoad == 0)
        {
            mainTheme.clip = mainThemeClip;
        }
        else if (levelToLoad == SceneManager.sceneCountInBuildSettings - 1)
        {
            mainTheme.clip = creditThemeClip;
        }
        else
        {
            mainTheme.clip = inGameThemeClip;  
        }


        mainTheme.Play();
        currentTime = 0;
        while (currentTime < (Constants.LEVEL_SWITCH_FADE_DURATION))
        {
            currentTime += Time.unscaledDeltaTime;
            mainTheme.volume =
                Mathf.Lerp(bottomvolume, start, currentTime / (Constants.LEVEL_SWITCH_FADE_DURATION));
            yield return null;
        }

        GameLauncher.WriteSettings();
    }

    public void LaunchLevelSelection()
    {
        int indexToChoose = SceneManager.GetActiveScene().buildIndex;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            indexToChoose--;
        }

        SelectLevel(_levelContainerButtons[indexToChoose]);
        _levelContainerButtons[indexToChoose].Select();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_levelContainerButtons[indexToChoose]
            .gameObject);
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
            if (!LevelCompletionTracker.unlockedLevels.Contains(i + 1))
            {
                _levelContainerButtons[i].interactable = false;
                Color32 color = _levelContainerButtons[i].gameObject.transform.GetComponentInChildren<TMP_Text>()
                    .faceColor;
                _levelContainerButtons[i].gameObject.transform.GetComponentInChildren<TMP_Text>().faceColor =
                    new Color32(color.r, color.b, color.g, disabledLevelTextAlpha);
            }
        }
    }

    public IEnumerator StartFadeToBlack(int sceneToLoad, float duration, bool turnBlack)
    {
        var b = black.GetComponent<Image>();
        var color = b.color;
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float val;
            if (turnBlack)
            {
                val = Mathf.Lerp(0, 1, currentTime / duration);
            }
            else
            {
                val = Mathf.Lerp(1, 0, currentTime / duration);
            }

            Color c = new Color(color.r, color.g, color.b, val);
            b.color = c;
            yield return null;
        }

        GameController.PlayerIsDead = false;
        if (turnBlack)
        {
            GameController.previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
            GameLauncher.WriteSettings();
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                GameController.SetInputLockState(false);
                FindObjectOfType<IngameMenu>().Unpause();
                LevelCompletionTracker.AddUnlockedLevel(sceneToLoad);
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    private void OnLevelSelectorPlayPressed()
    {
        if (LevelCompletionTracker.unlockedLevels.Contains(_selectedLevel))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                StartCoroutine(StartFade(_selectedLevel));
            }

            StartCoroutine(StartFadeToBlack(_selectedLevel, Constants.LEVEL_SWITCH_FADE_DURATION, true));
        }
    }

    private void MoveScrollView()
    {
        /*
        * Credit: https://forum.unity.com/threads/scrollview-using-controller-arrowkeys.1008121/
        */

        // Get the currently selected UI element from the event system.
        var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        // Return if there are none.
        if (selected == null)
        {
            return;
        }

        // Return if the selected game object is not inside the scroll rect.
        if (selected.transform.parent != scrollviewParent.transform)
        {
            return;
        }

        if (IsImageFullyVisible(selected))
        {
            return;
        }

        // Return if the selected game object is the same as it was last frame,
        // meaning we haven't moved.
        if (selected == lastSelected)
        {
            return;
        }


        // Get the rect tranform for the selected game object.
        selectedButton = selected.GetComponent<RectTransform>();
        SelectLevel(selected.GetComponent<Button>());
        // The position of the selected UI element is the absolute anchor position,
        // ie. the local position within the scroll rect + its height if we're
        // scrolling down. If we're scrolling up it's just the absolute anchor position.
        float selectedPositionY =
            Mathf.Abs(selectedButton.anchoredPosition.y) + selectedButton.rect.height;
        // The upper bound of the scroll view is the anchor position of the content we're scrolling.


        float scrollViewMinY = scrollviewParent.GetComponent<RectTransform>().anchoredPosition.y;
        // The lower bound is the anchor position + the height of the scroll rect.
        float scrollViewMaxY = scrollviewParent.GetComponent<RectTransform>().anchoredPosition.y +
                               scrollRectTransform.rect.height;
        // If the selected position is below the current lower bound of the scroll view we scroll down.
        if (selectedPositionY > scrollViewMaxY)
        {
            float newY = selectedPositionY - scrollRectTransform.rect.height;
            scrollviewParent.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(scrollviewParent.GetComponent<RectTransform>().anchoredPosition.x, newY);
        }
        // If the selected position is above the current upper bound of the scroll view we scroll up.
        else if (Mathf.Abs(selectedButton.anchoredPosition.y) < scrollViewMinY)
        {
            scrollviewParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                scrollviewParent.GetComponent<RectTransform>().anchoredPosition.x,
                Mathf.Abs(selectedButton.anchoredPosition.y));
        }

        lastSelected = selected;
    }


    void Update()
    {
        MoveScrollView();
    }

    private bool IsImageFullyVisible(GameObject obj)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        var image = obj.GetComponent<Image>();
        var bounds = new Bounds(image.transform.localPosition, image.rectTransform.rect.size);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
}