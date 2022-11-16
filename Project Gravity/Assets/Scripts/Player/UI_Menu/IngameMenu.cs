using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus;
    [SerializeField] public GameObject interactText;
    [SerializeField] private Texture2D customCursor;
    [SerializeField] private int previousMenu;
    [SerializeField] private TMP_Text levelRecordText;

    [SerializeField] private GameObject[] optionTabs;
    [SerializeField] private Button[] optionButtons;

    [Header("Volume settings game objects")] [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] private Slider dialogueVolumeSlider;

    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text effectsVolumeText;
    [SerializeField] private TMP_Text dialogueVolumeText;

    [Header("Speed settings game objects")] [SerializeField]
    private Slider speedSlider;

    [SerializeField] private TMP_Text speedText;
    private static Guid _playerDeathGuid;
    private static Guid _playerSucceedsGuid;
    public AudioMixer globalMixer;
    private UnityEngine.InputSystem.PlayerInput _playerInput;

    [Header("Tutorial toggle")] [SerializeField]
    private Toggle tutorialToggle;

    [Header("Controls")] [SerializeField] private TMP_Text currentControlScheme;
    [SerializeField] private Slider controlSchemeSlider;


    public void ToggleActionMap(bool paused)
    {
        if (paused)
        {
            if (_playerInput != null)
            {
                _playerInput.SwitchCurrentActionMap("MenuControls");
            }
        }
        else
        {
            if (_playerInput != null)
            {
                _playerInput.SwitchCurrentActionMap("PlayerControls");
            }
        }
    }

    public void OnTutorialToggleValueChanged()
    {
        GameController.TutorialIsOn = tutorialToggle.isOn;
    }

    private void Start()
    {
        EventSystem.Current.RegisterListener<WinningEvent>(OnPlayerSucceedsLevel, ref _playerSucceedsGuid);
        EventSystem.Current.RegisterListener<PlayerDeathEvent>(OnPlayerDeath, ref _playerDeathGuid);

        // Volume setup
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeValueChanged(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeValueChanged(); });
        effectsVolumeSlider.onValueChanged.AddListener(delegate { OnEffectsVolumeValueChanged(); });
        dialogueVolumeSlider.onValueChanged.AddListener(delegate { OnDialogueVolumeValueChanged(); });

        masterVolumeSlider.value = GameController.MasterVolumeMultiplier;
        musicVolumeSlider.value = GameController.MusicVolumeMultiplier;
        effectsVolumeSlider.value = GameController.EffectsVolumeMultiplier;
        dialogueVolumeSlider.value = GameController.DialogueVolumeMultiplier;

        masterVolumeText.text = Mathf.Round(masterVolumeSlider.value * 100.0f) + "%";
        musicVolumeText.text = Mathf.Round(musicVolumeSlider.value * 100.0f) + "%";
        effectsVolumeText.text = Mathf.Round(effectsVolumeSlider.value * 100.0f) + "%";
        dialogueVolumeText.text = Mathf.Round(dialogueVolumeSlider.value * 100.0f) + "%";


        // Speed
        speedSlider.onValueChanged.AddListener(delegate { OnSpeedValueChanged(); });
        speedSlider.value = GameController.GlobalSpeedMultiplier * 100;
        speedText.text = (speedSlider.value).ToString(CultureInfo.InvariantCulture) + "%";
        // Tutorial
        tutorialToggle.isOn = GameController.TutorialIsOn;
        tutorialToggle.onValueChanged.AddListener(delegate { OnTutorialToggleValueChanged(); });

        // Controls
        controlSchemeSlider.onValueChanged.AddListener(delegate { OnInputSchemeChanged(); });
        controlSchemeSlider.value = GameController.CurrentControlSchemeIndex;

        _playerInput = FindObjectOfType<UnityEngine.InputSystem.PlayerInput>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }

        if (FindObjectOfType<LevelSettings>().IsTutorialLevel() && GameController.TutorialIsOn)
        {
            _playerInput.SwitchCurrentActionMap("MenuControls");
        }
        else
        {
            _playerInput.SwitchCurrentActionMap("PlayerControls");
        }


        if (customCursor != null)
        {
            SetCustomCursor();
        }
    }

    public void OnMasterVolumeValueChanged()
    {
        GameController.MasterVolumeMultiplier = masterVolumeSlider.value;
        globalMixer.SetFloat("Master", Mathf.Log(GameController.MasterVolumeMultiplier) * 20f);
        masterVolumeText.text = Mathf.Round(masterVolumeSlider.value * 100.0f) + "%";
    }

    public void OnMusicVolumeValueChanged()
    {
        GameController.MusicVolumeMultiplier = musicVolumeSlider.value;
        globalMixer.SetFloat("Music", Mathf.Log(GameController.MusicVolumeMultiplier) * 20f);
        musicVolumeText.text = Mathf.Round(musicVolumeSlider.value * 100.0f) + "%";
    }

    public void OnEffectsVolumeValueChanged()
    {
        GameController.EffectsVolumeMultiplier = effectsVolumeSlider.value;
        globalMixer.SetFloat("Effects", Mathf.Log(GameController.EffectsVolumeMultiplier) * 20f);
        effectsVolumeText.text = Mathf.Round(effectsVolumeSlider.value * 100.0f) + "%";
    }

    public void OnDialogueVolumeValueChanged()
    {
        GameController.DialogueVolumeMultiplier = dialogueVolumeSlider.value;
        globalMixer.SetFloat("Dialogue", Mathf.Log(GameController.DialogueVolumeMultiplier) * 20f);
        dialogueVolumeText.text = Mathf.Round(dialogueVolumeSlider.value * 100.0f) + "%";
    }

    private void OnSpeedValueChanged()
    {
        GameController.GlobalSpeedMultiplier = speedSlider.value / 100;
        GravityController.SetNewGravity(GravityController.GetCurrentFacing());
        speedText.text = (speedSlider.value).ToString(CultureInfo.InvariantCulture) + "%";
    }
    
    
    public void OnInputSchemeChanged()
    {
        GameController.CurrentControlSchemeIndex = (int) controlSchemeSlider.value;
        if (GameController.CurrentControlSchemeIndex == 0)
        {
            // TODO 
            // Set input to mouse and keyboard
            currentControlScheme.text = "Mouse & Keyboard";
            
        }
        else
        {
            // TODO 
            // Set input to controller
            currentControlScheme.text = "Controller";
        }
        
    }

    void SetCustomCursor()
    {
        Cursor.SetCursor(customCursor, new Vector2(customCursor.width / 2, customCursor.height / 2), CursorMode.Auto);
    }

    public void ChangePauseState(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Debug.Log("Nice try David");
                return;
            }

            if (menus[0].gameObject.activeSelf)
            {
                Unpause();
                return;
            }

            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].activeSelf)
                {
                    return;
                }
            }

            Pause(0);
        }
    }

    public void ClosePauseScreen(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (gameObject.transform.GetChild(0).gameObject.activeSelf && menus[0].gameObject.activeSelf)
            {
                Unpause();
            }
        }
    }

    public void Unpause()
    {
        if (menus.Length > 0)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].activeSelf)
                {
                    menus[i].SetActive(false);
                }
            }
        }

        if (gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        ToggleActionMap(false);
        SetCustomCursor();
        GameController.UnpauseGame();
    }

    public void Pause(int index)
    {
        gameObject.transform.parent.GetComponent<AudioSource>().mute = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(true);
        if (!menus[index].activeSelf)
        {
            menus[index].SetActive(true);
        }

        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1 && index == 1)
        {
            if (menus[index].transform.GetChild(0).GetComponent<Button>().IsInteractable())
            {
                menus[index].transform.GetChild(0).GetComponent<Button>().interactable = false;
            }
            else
            {
                menus[index].transform.GetChild(0).GetComponent<Button>().interactable = true;
            }
        }

        ToggleActionMap(true);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        GameController.PauseGame();
    }

    private void OnPlayerDeath(PlayerDeathEvent playerDeathEvent)
    {
        StartCoroutine(RestartWhenDead(playerDeathEvent));
    }

    private IEnumerator RestartWhenDead(PlayerDeathEvent playerDeathEvent)
    {
        GameController.PauseGame();
        yield return new WaitForSecondsRealtime(playerDeathEvent.DeathTime);
        Restart();
    }

    public void OnPlayerSucceedsLevel(WinningEvent winningEvent)
    {
        LevelCompletionTracker.SetLevelBest(SceneManager.GetActiveScene().buildIndex,
            FindObjectOfType<LevelTimer>().GetTimePassed());

        float bestTime = LevelCompletionTracker.levelRecords[SceneManager.GetActiveScene().buildIndex];

        float minutes = Mathf.FloorToInt(bestTime / 60);
        float seconds = Mathf.FloorToInt(bestTime % 60);
        float milliSeconds = Mathf.Floor(bestTime % 1 * 100);
        levelRecordText.text = $"{minutes:00}:{seconds:00}:{milliSeconds:00}";
        Pause(1);
    }

    public void LoadScene(int scene)
    {
        if (scene == 0)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            SetCustomCursor();
        }

        Unpause();
        LevelCompletionTracker.AddCompletedLevel(scene);
        SceneManager.LoadScene(scene);
    }


    public void Restart()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 1)
        {
            return;
        }

        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousScene()
    {
        if (SceneManager.GetActiveScene().buildIndex <= 1)
        {
            return;
        }

        LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenLevelSelector(int index)
    {
        previousMenu = index;
        for (int i = 0; i < menus.Length - 1; i++)
        {
            if (menus[i].activeSelf)
            {
                menus[i].SetActive(false);
            }
        }

        if (!menus[3].activeSelf)
        {
            menus[3].SetActive(true);
        }

        for (int i = 0; i < menus[3].GetComponentsInChildren<Button>().Length - 1; i++)
        {
            if (i >= SceneManager.sceneCountInBuildSettings - 1)
            {
                menus[3].transform.GetChild(i).GetComponent<Button>().interactable = false;
                continue;
            }

            if (LevelCompletionTracker.unlockedLevels.Count < 1)
            {
                LevelCompletionTracker.AddCompletedLevel(1);
            }

            if (!FindObjectOfType<LevelSettings>().GetLevelsAreUnlocked())
            {
                if (!LevelCompletionTracker.unlockedLevels.Contains(i + 1))
                {
                    menus[3].transform.GetChild(i).GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    public void OpenOptionsMenu(int index)
    {
        previousMenu = index;
        for (int i = 0; i < menus.Length - 1; i++)
        {
            if (menus[i].activeSelf)
            {
                menus[i].SetActive(false);
            }
        }

        if (!menus[4].activeSelf)
        {
            menus[4].SetActive(true);
        }

        OnOptionTabButtonClick(0);
    }

    public void CloseOptionsMenu()
    {
        if (menus[4].activeSelf)
        {
            menus[4].SetActive(false);
        }

        Pause(previousMenu);
    }

    public void CloseLevelSelector()
    {
        if (menus[3].activeSelf)
        {
            menus[3].SetActive(false);
        }

        Pause(previousMenu);
    }

    public void OnOptionTabButtonClick(int index)
    {
        foreach (var go in optionTabs)
        {
            go.SetActive(false);
        }

        //optionButtons[index].Select();
        optionTabs[index].SetActive(true);
    }
}