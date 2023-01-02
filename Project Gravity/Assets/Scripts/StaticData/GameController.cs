using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameController
{
    private static bool _inputLocked = true;
    public static int CurrentControlSchemeIndex = 0;

    public static int previousSceneIndex;
    // Sound
    public static bool GlobalSoundIsOn;
    public static float MasterVolumeMultiplier;
    public static float MusicVolumeMultiplier;
    public static float EffectsVolumeMultiplier;
    public static float DialogueVolumeMultiplier;

    // Game
    public static int FullscreenMode;
    public static bool SlowSpeedToggled;
    public static bool TutorialIsOn;
    public static bool DyslecticModeIsOn;

    // Player death
    public static bool PlayerIsDead;
    
    // Camera rotation toggle
    public static bool CameraAutoRotationToggled;
    
    // Player
    public static float PlayerAcceleration;
    public static float PlayerJumpForce;
    public static float PlayerAirMovementMultiplier;
    public static float PlayerMaxVelocity;

    public static void SetUpSettings()
    {
        // Sound
        GlobalSoundIsOn = true;
        MasterVolumeMultiplier = 1f;
        MusicVolumeMultiplier = 1f;
        EffectsVolumeMultiplier = 1f;
        DialogueVolumeMultiplier = 1f;
        
        // Game
        FullscreenMode = 0;
        SlowSpeedToggled = false;
        TutorialIsOn = true;
        CameraAutoRotationToggled = false;
        DyslecticModeIsOn = false;
    }

    public static void SetUpNormalSpeed()
    {
        // Player
        PlayerAcceleration = 10f;
        PlayerJumpForce = 8f;
        PlayerAirMovementMultiplier = 0.7f;
        PlayerMaxVelocity = 5f;
    }

    public static void SetUpSlowSpeed()
    {
        // Player
        PlayerAcceleration = 10f;
        PlayerJumpForce = 7f;
        PlayerAirMovementMultiplier = 1f;
        PlayerMaxVelocity = 4f;
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
