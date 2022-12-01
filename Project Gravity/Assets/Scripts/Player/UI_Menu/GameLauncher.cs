using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public static class GameLauncher
{
    public static readonly string levelSettingsFile = Application.persistentDataPath + "/levelData.data";
    public static readonly string settingsFile = Application.persistentDataPath + "/settingsData.data";
    public static readonly string settingsTextFile = Application.persistentDataPath + "/Settings.txt";

    // Used for writing and reading level and settings data.
    private static FileStream fileStream;
    private static BinaryFormatter converter = new BinaryFormatter();

    private static SettingsData _settingsData;
    private static LevelData _levelData;
    private static ManualSettingsData _manualSettingsData;

    public static void SaveSettings()
    {
        _settingsData = new SettingsData();
        _levelData = new LevelData();
        WriteData(settingsFile, _settingsData);
        WriteData(levelSettingsFile, _levelData);
        WriteSettings();
    }

    private static void WriteData<T>(string filePath, T data)
    {
        if (File.Exists(filePath))
        {
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open);
                converter.Serialize(fileStream, data);
                fileStream.Close();
            }
            catch (Exception e)
            {
                fileStream?.Close();
                File.Delete(filePath);
                fileStream = new FileStream(filePath, FileMode.Create);
                converter.Serialize(fileStream, data);
                fileStream.Close();
            }
        }
        else
        {
            fileStream = new FileStream(filePath, FileMode.Create);
            converter.Serialize(fileStream, data);
            fileStream.Close();
        }
    }

    private static void WriteSettings()
    {
        // If the settings file doesn't exist
        if (!File.Exists(settingsTextFile))
        {
            _manualSettingsData = new ManualSettingsData();
            fileStream = new FileStream(settingsTextFile, FileMode.Create);
            fileStream.Dispose();
            File.AppendAllText(settingsTextFile, _manualSettingsData.ToString());
        }
    }

    public static void LoadSettings()
    {
        if (File.Exists(settingsFile))
        {
            try
            {
                fileStream = new FileStream(settingsFile, FileMode.Open);
                _settingsData = converter.Deserialize(fileStream) as SettingsData;
                fileStream.Close();
                GameController.SetUp(_settingsData);
            }
            catch (Exception e)
            {
                fileStream?.Close();
                File.Delete(settingsFile);
                GameController.SetUp();
            }
        }
        else
        {
            GameController.SetUp();
        }


        if (File.Exists(levelSettingsFile))
        {
            try
            {
                fileStream = new FileStream(levelSettingsFile, FileMode.Open);
                _levelData = converter.Deserialize(fileStream) as LevelData;
                fileStream.Close();
                LevelCompletionTracker.LoadLevels(_levelData);
            }
            catch (Exception e)
            {
                fileStream?.Close();
                File.Delete(levelSettingsFile);
            }
        }

        if (File.Exists(settingsTextFile))
        {
            // Number of setting options
            float[] floats = new float[4];
            int counter = 0;
            foreach (var line in File.ReadLines(settingsTextFile))
            {
                floats[counter] = float.Parse(line.Split(' ', 2)[1].Trim());
                counter++;
            }

            ManualSettingsData manualSettingsData = new ManualSettingsData(floats[0], floats[1], floats[2], floats[3]);

            PlayerStaticValues.PlayerJumpMultiplier = manualSettingsData.JumpMultiplier;
            PlayerStaticValues.PlayerMovementMultiplier = manualSettingsData.MovementMultiplier;
            PlayerStaticValues.PlayerAirMovementMultiplier = manualSettingsData.AirMovementMultiplier;
            GravityController.GRAVITY = Constants.GRAVITY * manualSettingsData.GravityMultiplier;
        }
    }
}

[Serializable]
public class ManualSettingsData
{
    public float JumpMultiplier { get; set; }
    public float MovementMultiplier{ get; set; }
    public float GravityMultiplier { get; set; }
    public float AirMovementMultiplier{ get; set; }

    public ManualSettingsData(float jMultiplier, float mMultiplier, float gMultiplier, float aMovementMultiplier)
    {
        JumpMultiplier = jMultiplier;
        MovementMultiplier = mMultiplier;
        GravityMultiplier = gMultiplier;
        AirMovementMultiplier = aMovementMultiplier;
    }

    public ManualSettingsData()
    {
        JumpMultiplier = 1;
        MovementMultiplier = 1;
        GravityMultiplier = 1;
        AirMovementMultiplier = 1;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Jump: " + JumpMultiplier + "\n");
        sb.Append("Movement: " + MovementMultiplier + "\n");
        sb.Append("Gravity: " + GravityMultiplier + "\n");
        sb.Append("Air: " + AirMovementMultiplier);

        return sb.ToString();
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