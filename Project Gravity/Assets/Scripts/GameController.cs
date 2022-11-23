using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameController
{
    private static bool _inputLocked = true;
    public static int CurrentControlSchemeIndex = 0;
    // Sound
    public static bool GlobalSoundIsOn;
    public static float MasterVolumeMultiplier;
    public static float MusicVolumeMultiplier;
    public static float EffectsVolumeMultiplier;
    public static float DialogueVolumeMultiplier;

    // Game
    public static int FullscreenMode;
    public static float GlobalSpeedMultiplier;
    public static bool TutorialIsOn;

    public static bool playerIsDead;

    public static void SetUp(SettingsData settingsData)
    {
        // Sound
        GlobalSoundIsOn = settingsData.SoundIsOn;
        MasterVolumeMultiplier = settingsData.MasterVolumeMultiplier;
        MusicVolumeMultiplier = settingsData.MusicVolumeMultiplier;
        EffectsVolumeMultiplier = settingsData.EffectsVolumeMultiplier;
        DialogueVolumeMultiplier = settingsData.DialogueVolumeMultiplier;
        
        // Game
        FullscreenMode = settingsData.ScreenMode;
        GlobalSpeedMultiplier = settingsData.GlobalSpeedMultiplier;
        TutorialIsOn = settingsData.TutorialIsOn;
    }

    public static void SetUp()
    {
        // Sound
        GlobalSoundIsOn = true;
        MasterVolumeMultiplier = 1f;
        MusicVolumeMultiplier = 1f;
        EffectsVolumeMultiplier = 1f;
        DialogueVolumeMultiplier = 1f;
        
        // Game
        FullscreenMode = 0;
        GlobalSpeedMultiplier = 1f;
        TutorialIsOn = true;
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
        SetInputLockState(true);
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1;
        SetInputLockState(false);
    }

    // Added for tutorial
    public static bool IsPaused()
    {
        return Time.timeScale == 0;
    }

    public static void SetInputLockState(bool locked)
    {
        _inputLocked = locked;
    }

    public static bool GetPlayerInputIsLocked()
    {
        return _inputLocked;
    }
}
