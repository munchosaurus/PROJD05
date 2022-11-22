using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour
{
    [Header("Speed settings")] [SerializeField]
    private Slider speedSlider;

    [SerializeField] private TMP_Text speedText;
    private static Guid _playerDeathGuid;
    private static Guid _playerSucceedsGuid;

    [Header("Tutorial toggle")] [SerializeField]
    private Toggle tutorialToggle;
    
    [Header("Screen mode")] [SerializeField]
    private TMP_Dropdown fullscreenDropdown;

    // Start is called before the first frame update
    void Start()
    {
        // Speed
        speedSlider.onValueChanged.AddListener(delegate { OnSpeedValueChanged(); });
        speedSlider.value = GameController.GlobalSpeedMultiplier * 100;
        speedText.text = (speedSlider.value).ToString(CultureInfo.InvariantCulture) + "%";

        // Tutorial
        tutorialToggle.onValueChanged.AddListener(delegate { OnTutorialToggleValueChanged(); });
        tutorialToggle.isOn = GameController.TutorialIsOn;

        // Screen mode
        fullscreenDropdown.onValueChanged.AddListener(delegate { OnFullScreenToggleChanged(); });
        fullscreenDropdown.value = GameController.FullscreenMode;
    }

    public void LoadGameSettings()
    {
        speedSlider.value = GameController.GlobalSpeedMultiplier * 100;
        speedText.text = (speedSlider.value).ToString(CultureInfo.InvariantCulture) + "%";
        tutorialToggle.isOn = GameController.TutorialIsOn;
        fullscreenDropdown.value = GameController.FullscreenMode;
    }
    
    public void OnTutorialToggleValueChanged()
    {
        GameController.TutorialIsOn = tutorialToggle.isOn;
    }

    public void OnFullScreenToggleChanged()
    {
        GameController.FullscreenMode = fullscreenDropdown.value;

        switch (fullscreenDropdown.value)
        {
            case 0:
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                Screen.fullScreen = false;
                break;
            case 2:
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }
    }
    
    private void OnSpeedValueChanged()
    {
        GameController.GlobalSpeedMultiplier = speedSlider.value / 100;
        GravityController.SetNewGravity(GravityController.GetCurrentFacing());
        speedText.text = (speedSlider.value).ToString(CultureInfo.InvariantCulture) + "%";
    }

}
