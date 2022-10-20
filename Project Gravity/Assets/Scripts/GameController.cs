using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameController
{
    private static bool _inputLocked;
    
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

    public static void SetInputLockState(bool locked)
    {
        _inputLocked = locked;
    }

    public static bool GetPlayerInputIsLocked()
    {
        return _inputLocked;
    }
}
