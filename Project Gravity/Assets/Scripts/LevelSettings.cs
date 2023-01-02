using System;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [Header("Toggle this on if you want the level to use default values for camera")] [SerializeField]
    private bool isDefault;

    [Header("Camera settings, only to be altered in case of the lvel not using default values")] [SerializeField]
    private float leeway;

    [SerializeField] private float smoothTime;
    [SerializeField] private float minX, maxX, minY, maxY;
    [SerializeField] private float maximumDistance;
    [SerializeField] private float minimumDistance;
    [SerializeField] private float targetX;
    [SerializeField] private float targetY;
    [SerializeField] private float targetZ;

    [Header("Time settings")] [SerializeField]
    private bool isTimed;

    [SerializeField] private float timeLimit;
    [SerializeField] private float silverTimeMinimum;
    [SerializeField] private float silverTimeMaximum;

    [Header("Key card settings")] [SerializeField]
    private bool keycardsExistInLevel;

    [SerializeField] private int numberOfKeyCards;

    [Header("Level lock settings")] [SerializeField]
    private bool unlockAllLevels;

    [SerializeField] private bool isTutorialLevel;


    void Awake()
    {
        if (isDefault)
        {
            SetDefaultCameraValues();
        }
        Physics.gravity = new Vector3(0, -GravityController.GravityMultiplier, 0);
    }

    void SetDefaultCameraValues()
    {
        leeway = Constants.LEVEL_DEFAULT_CAMERA_LEEWAY;
        smoothTime = Constants.LEVEL_DEFAULT_CAMERA_MOVEMENT_SMOOTHTIME;
        minX = Constants.LEVEL_DEFAULT_CAMERA_MINIMUM_X;
        maxX = Constants.LEVEL_DEFAULT_CAMERA_MAXIMUM_X;
        minY = Constants.LEVEL_DEFAULT_CAMERA_MINIMUM_Y;
        maxY = Constants.LEVEL_DEFAULT_CAMERA_MAXIMUM_Y;
        maximumDistance = Constants.LEVEL_DEFAULT_CAMERA_MAXIMUM_DISTANCE;
        minimumDistance = Constants.LEVEL_DEFAULT_CAMERA_MINIMUM_DISTANCE;
        targetX = Constants.LEVEL_DEFAULT_CAMERA_TARGET_X;
        targetY = Constants.LEVEL_DEFAULT_CAMERA_TARGET_Y;
        targetZ = Constants.LEVEL_DEFAULT_CAMERA_TARGET_Z;

    }

    public bool IsTutorialLevel()
    {
        return isTutorialLevel;
    }
    
    public float GetLevelLeeway()
    {
        return leeway;
    }

    public float GetLevelSmoothTime()
    {
        return smoothTime;
    }

    public float GetLevelMinX()
    {
        return minX;
    }

    public float GetLevelMaxX()
    {
        return maxX;
    }

    public float GetLevelMinY()
    {
        return minY;
    }

    public float GetLevelMaxY()
    {
        return maxY;
    }

    public float GetLevelCameraMaximumDistance()
    {
        return maximumDistance;
    }

    public float GetLevelCameraMinimumDistance()
    {
        return minimumDistance;
    }

    public float GetLevelXTargetValue()
    {
        return targetX;
    }

    public float GetLevelYTargetValue()
    {
        return targetY;
    }

    public float GetLevelZTargetValue()
    {
        return targetZ;
    }

    public bool GetLevelIsTimed()
    {
        return isTimed;
    }

    public float GetLevelSilverMinimum()
    {
        return silverTimeMinimum;
    }

    public float GetLevelSilverMaximum()
    {
        return silverTimeMaximum;
    }

    public float GetLevelTimeLimit()
    {
        return timeLimit;
    }

    public bool KeycardsExistInLevel()
    {
        return keycardsExistInLevel;
    }

    public int GetNumberOfKeycardsInLevel()
    {
        return numberOfKeyCards;
    }

    public bool GetLevelsAreUnlocked()
    {
        return unlockAllLevels;
    }
}
