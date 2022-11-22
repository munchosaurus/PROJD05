using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameLauncher
{
    public static readonly string levelSettingsFile = Application.persistentDataPath + "/levelData.data";

    public static readonly string settingsFile = Application.persistentDataPath + "/settingsData.data";

    // Used for writing and reading level and settings data.
    private static FileStream fileStream;
    private static BinaryFormatter converter = new BinaryFormatter();

    private static SettingsData _settingsData;
    private static LevelData _levelData;

    public static void SaveSettings()
    {
        _settingsData = new SettingsData();
        _levelData = new LevelData();
        WriteData(settingsFile, _settingsData);
        WriteData(levelSettingsFile, _levelData);
    }

    private static void WriteData<T>(string filePath, T data)
    {
        if (File.Exists(filePath))
        {
            fileStream = new FileStream(filePath, FileMode.Open);
            converter.Serialize(fileStream, data);
            fileStream.Close();
        }
        else
        {
            fileStream = new FileStream(filePath, FileMode.Create);
            converter.Serialize(fileStream, data);
            fileStream.Close();
        }
    }

    public static void LoadSettings()
    {
        if (File.Exists(settingsFile))
        {
            fileStream = new FileStream(settingsFile, FileMode.Open);
            _settingsData = converter.Deserialize(fileStream) as SettingsData;
            fileStream.Close();
            GameController.SetUp(_settingsData);
        }
        else
        {
            GameController.SetUp();
        }

        if (File.Exists(levelSettingsFile))
        {
            fileStream = new FileStream(levelSettingsFile, FileMode.Open);
            _levelData = converter.Deserialize(fileStream) as LevelData;
            fileStream.Close();
            LevelCompletionTracker.LoadLevels(_levelData);
        }
    }
}


[Serializable]
public class SettingsData
{
    public SettingsData()
    {
        // Volume
        SoundIsOn = GameController.GlobalSoundIsOn;
        MasterVolumeMultiplier = GameController.MasterVolumeMultiplier;
        MusicVolumeMultiplier = GameController.MusicVolumeMultiplier;
        EffectsVolumeMultiplier = GameController.EffectsVolumeMultiplier;
        DialogueVolumeMultiplier = GameController.DialogueVolumeMultiplier;

        // Game
        ScreenMode = GameController.FullscreenMode;
        TutorialIsOn = GameController.TutorialIsOn;
        GlobalSpeedMultiplier = GameController.GlobalSpeedMultiplier;
    }

    // Volume
    public bool SoundIsOn { get; set; }
    public float MasterVolumeMultiplier { get; set; }
    public float MusicVolumeMultiplier { get; set; }
    public float EffectsVolumeMultiplier { get; set; }
    public float DialogueVolumeMultiplier { get; set; }

    // Game
    public int ScreenMode { get; set; }
    public bool TutorialIsOn { get; set; }
    public float GlobalSpeedMultiplier { get; set; }
}

[Serializable]
public class LevelData
{
    public LevelData()
    {
        LevelRecords = LevelCompletionTracker.levelRecords;
    }

    public Dictionary<int, float> LevelRecords { get; set; }
}