using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameController
{
    private static bool _inputLocked = true;
    public static float MasterVolumeMultiplier = 1f;
    public static float MusicVolumeMultiplier = 1f;
    public static float EffectsVolumeMultiplier = 1f;
    public static float DialogueVolumeMultiplier = 1f;
    public static int CurrentControlSchemeIndex = 0;
    
    public static float GlobalSpeedMultiplier = 1f;
    public static bool TutorialIsOn = true;

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
