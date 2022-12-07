using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Mono.Collections.Generic;
using TMPro;
using UnityEditor;
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

    [Header("Camera Rotation toggle")] [SerializeField] private Toggle cameraAutoRotationToggle;
    
    [Header("Screen mode")] [SerializeField]
    private TMP_Dropdown fullscreenDropdown;

    [Header("Dyslectic mode")]
    [SerializeField] private Toggle dyslecticToggle;
    [SerializeField] private TMP_FontAsset dyslecticFont;
    [SerializeField] private TMP_FontAsset regularFont;
    
    
    

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
        
        // Camera rotation
        cameraAutoRotationToggle.onValueChanged.AddListener(delegate { OnCameraRotationToggleChanged(); });
        cameraAutoRotationToggle.isOn = GameController.CameraAutoRotationToggled;
        
        // Dyslecticfont
        dyslecticToggle.onValueChanged.AddListener(delegate { OnDyslecticToggleChanged(); });
        dyslecticToggle.isOn = GameController.DyslecticModeIsOn;
        if (GameController.DyslecticModeIsOn)
        {
            SetFont(dyslecticFont);
        }
    }

    public void LoadGameSettings()
    {
        speedSlider.value = GameController.GlobalSpeedMultiplier * 100;
        speedText.text = (speedSlider.value).ToString(CultureInfo.InvariantCulture) + "%";
        tutorialToggle.isOn = GameController.TutorialIsOn;
        fullscreenDropdown.value = GameController.FullscreenMode;
        cameraAutoRotationToggle.isOn = GameController.CameraAutoRotationToggled;
        dyslecticToggle.isOn = GameController.DyslecticModeIsOn;
        OnFullScreenToggleChanged();
    }

    public void OnDyslecticToggleChanged()
    {
        GameController.DyslecticModeIsOn = dyslecticToggle.isOn;
        if (GameController.DyslecticModeIsOn)
        {
            SetFont(dyslecticFont);
        }
        else
        {
            SetFont(regularFont);
        }
        GameLauncher.WriteSettings();
    }
    
    public void SetFont(TMP_FontAsset fontToUse)
    {
        Collection<TMP_Text> texts = new Collection<TMP_Text>();
        foreach (TMP_Text go in Resources.FindObjectsOfTypeAll(typeof(TMP_Text)) as TMP_Text[])
        {
            if (go.hideFlags != HideFlags.None)
                continue;
            if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab || PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
                continue;
            texts.Add(go);
        }
        foreach (var VARIABLE in texts)
        {
            VARIABLE.font = fontToUse;
        }
    } 

    public void OnCameraRotationToggleChanged()
    {
        GameController.CameraAutoRotationToggled = cameraAutoRotationToggle.isOn;
        
        GameLauncher.WriteSettings();
    }
    
    public void OnTutorialToggleValueChanged()
    {
        GameController.TutorialIsOn = tutorialToggle.isOn;
        
        GameLauncher.WriteSettings();
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
        
        GameLauncher.WriteSettings();;
    }
    
    private void OnSpeedValueChanged()
    {
        GameController.GlobalSpeedMultiplier = speedSlider.value / 100;
        GravityController.SetNewGravity(GravityController.GetCurrentFacing());
        speedText.text = (speedSlider.value).ToString(CultureInfo.InvariantCulture) + "%";
        
        GameLauncher.WriteSettings();;
    }

}
