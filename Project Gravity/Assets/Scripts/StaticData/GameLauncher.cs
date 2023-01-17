using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public static class GameLauncher
{
    public static readonly string levelSettingsFile = Application.persistentDataPath + "/levelData.data";
    public static readonly string settingsTextFile = Application.persistentDataPath + "/Settings.txt";
    public static readonly string testSettingsTextFile = Application.persistentDataPath + "/TestSettings.txt";

    // Used for writing and reading level and settings data.
    private static FileStream _fileStream;
    private static BinaryFormatter converter = new BinaryFormatter();

    private static SettingsData _settingsData;
    private static LevelData _levelData;
    private static TestData _testData;

    public static void SaveLevels()
    {
        _levelData = new LevelData();
        WriteData(levelSettingsFile, _levelData);
    }

    private static void WriteData<T>(string filePath, T data)
    {
        if (File.Exists(filePath))
        {
            try
            {
                _fileStream = new FileStream(filePath, FileMode.Open);
                converter.Serialize(_fileStream, data);
                _fileStream.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _fileStream?.Close();
                File.Delete(filePath);
                _fileStream = new FileStream(filePath, FileMode.Create);
                converter.Serialize(_fileStream, data);
                _fileStream.Close();
            }
        }
        else
        {
            try
            {
                _fileStream = new FileStream(filePath, FileMode.Create);
                converter.Serialize(_fileStream, data);
                _fileStream.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    public static void WriteSettings()
    {
        _settingsData = new SettingsData();

        if (!File.Exists(settingsTextFile))
        {
            _fileStream = new FileStream(settingsTextFile, FileMode.Create);
            _fileStream.Dispose();
            File.AppendAllText(settingsTextFile, _settingsData.ToString());
        }
        else
        {
            try
            {
                File.WriteAllText(settingsTextFile, String.Empty);
                File.AppendAllText(settingsTextFile, _settingsData.ToString());
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _fileStream?.Close();
                File.Delete(settingsTextFile);
                _fileStream = new FileStream(settingsTextFile, FileMode.Create);
                _fileStream?.Dispose();
                File.AppendAllText(settingsTextFile, _settingsData.ToString());
            }
        }

        // TODO: REMOVE LATER
        _testData = new TestData();

        if (!File.Exists(testSettingsTextFile))
        {
            _fileStream = new FileStream(testSettingsTextFile, FileMode.Create);
            _fileStream.Dispose();
            File.AppendAllText(testSettingsTextFile, _testData.ToString());
        }
        else
        {
            try
            {
                File.WriteAllText(testSettingsTextFile, String.Empty);
                File.AppendAllText(testSettingsTextFile, _testData.ToString());
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _fileStream?.Close();
                File.Delete(testSettingsTextFile);
                _fileStream = new FileStream(testSettingsTextFile, FileMode.Create);
                _fileStream?.Dispose();
                File.AppendAllText(testSettingsTextFile, _testData.ToString());
            }
        }
    }

    private static void LoadLevelSettings()
    {
        if (File.Exists(levelSettingsFile))
        {
            try
            {
                _fileStream = new FileStream(levelSettingsFile, FileMode.Open);
                _levelData = converter.Deserialize(_fileStream) as LevelData;
                _fileStream.Close();
                LevelCompletionTracker.LoadLevels(_levelData);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _fileStream?.Close();
                File.Delete(levelSettingsFile);
            }
        }
    }

    private static void LoadTextSettings()
    {
        if (File.Exists(settingsTextFile))
        {
            int counter = 0;
            string[] values = new string[11];
            foreach (var line in File.ReadLines(settingsTextFile))
            {
                values[counter] = line.Split(' ', 2)[1].Trim();
                counter++;
            }

            UpdateSettings(values);
        }
        else
        {
            GameController.SetUpSettings();
        }
    }

    /*
     * TODO: Remove at launch
     */
    private static void LoadTestSettings()
    {
        if (File.Exists(testSettingsTextFile))
        {
            int counter = 0;
            string[] values = new string[5];
            foreach (var line in File.ReadLines(testSettingsTextFile))
            {
                values[counter] = line.Split(' ', 2)[1].Trim();
                counter++;
            }

            UpdateTestSettings(values);
        }
        else
        {
            GameController.SetUpNormalSpeed();
            GravityController.SetUpNormalSpeed();
            _testData = new TestData();
            _fileStream = new FileStream(testSettingsTextFile, FileMode.Create);
            _fileStream.Dispose();
            File.AppendAllText(testSettingsTextFile, _testData.ToString());
        }
    }

    public static void LoadSettings()
    {
        try
        {
            LoadLevelSettings();
            LoadTextSettings();
            LoadTestSettings();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void UpdateSettings(string[] values)
    {
        // Sound
        GameController.GlobalSoundIsOn = bool.Parse(values[0]);
        GameController.MasterVolumeMultiplier = float.Parse(values[1]) / 100f;
        GameController.MusicVolumeMultiplier = float.Parse(values[2]) / 100f;
        GameController.EffectsVolumeMultiplier = float.Parse(values[3]) / 100f;
        GameController.DialogueVolumeMultiplier = float.Parse(values[4]) / 100f;

        // Game
        GameController.FullscreenMode = int.Parse(values[5]);
        GameController.TutorialIsOn = bool.Parse(values[6]);
        GameController.SlowSpeedToggled = bool.Parse(values[7]);
        GameController.CameraAutoRotationToggled = bool.Parse(values[8]);
        GameController.DyslecticModeIsOn = bool.Parse(values[9]);

        // Control
        GameController.CurrentControlSchemeIndex = int.Parse(values[10]);
    }

    private static void UpdateTestSettings(string[] values)
    {
        // Movement
        GameController.PlayerJumpForce = float.Parse(values[0]);
        GameController.PlayerAcceleration = float.Parse(values[1]);
        GameController.PlayerAirMovementMultiplier = float.Parse(values[2]);
        GameController.PlayerMaxVelocity = float.Parse(values[3]);
        GravityController.GravityMultiplier = float.Parse(values[4]);
    }
}

[Serializable]
public class TestData
{
    public TestData()
    {
        JumpForce = GameController.PlayerJumpForce;
        Acceleration = GameController.PlayerAcceleration;
        AirMovementMultiplier = GameController.PlayerAirMovementMultiplier;
        MaxVelocity = GameController.PlayerMaxVelocity;
        GravityMultiplier = GravityController.GravityMultiplier;
    }

    // Movement
    public float JumpForce { get; set; }
    public float Acceleration { get; set; }
    public float AirMovementMultiplier { get; set; }
    public float MaxVelocity { get; set; }
    public float GravityMultiplier { get; set; }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        // Movement
        sb.Append("JumpForce: " + JumpForce + "\n");
        sb.Append("Acceleration: " + Acceleration + "\n");
        sb.Append("AirMovementMultiplier: " + AirMovementMultiplier + "\n");
        sb.Append("MaxVelocity: " + MaxVelocity + "\n");
        sb.Append("GravityMultiplier: " + GravityMultiplier);

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
        SlowSpeedToggled = GameController.SlowSpeedToggled;
        CameraAutoRotationToggled = GameController.CameraAutoRotationToggled;
        DyslecticModeIsOn = GameController.DyslecticModeIsOn;

        // Controll
        CurrentControlSchemeIndex = GameController.CurrentControlSchemeIndex;
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
    public bool SlowSpeedToggled { get; set; }
    public bool CameraAutoRotationToggled { get; set; }
    public bool DyslecticModeIsOn { get; set; }

    // Control
    public int CurrentControlSchemeIndex { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        // Sound
        sb.Append("SoundIsOn: " + SoundIsOn + "\n");
        sb.Append("MasterVolume: " + Mathf.Floor(MasterVolumeMultiplier * 100) + "\n");
        sb.Append("MusicVolume: " + Mathf.Floor(MusicVolumeMultiplier * 100) + "\n");
        sb.Append("EffectsVolume: " + Mathf.Floor(EffectsVolumeMultiplier * 100) + "\n");
        sb.Append("DialogueVolume: " + Mathf.Floor(DialogueVolumeMultiplier * 100) + "\n");

        // Game
        sb.Append("ScreenMode: " + ScreenMode + "\n");
        sb.Append("TutorialIsOn: " + TutorialIsOn + "\n");
        sb.Append("SlowSpeedToggled: " + SlowSpeedToggled + "\n");
        sb.Append("CameraAutoRotationToggled: " + CameraAutoRotationToggled + "\n");
        sb.Append("DyslecticModeIsOn: " + DyslecticModeIsOn + "\n");

        // Control
        sb.Append("CurrentControlSchemeIndex: " + CurrentControlSchemeIndex);

        return sb.ToString();
    }
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