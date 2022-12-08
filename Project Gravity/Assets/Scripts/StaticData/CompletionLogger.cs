﻿using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CompletionLogger
{
    public static readonly string logFile = Application.persistentDataPath + "/Logfile.csv";
    public static readonly string playCountFile = Application.persistentDataPath + "/Playcountfile.txt";
    private static CompletionLogLine _completionLogLine;
    private static FileStream _fileStream;
    public static int count;
    public static float finishTime;
    public static int gravityChanges;
    public static int win;
    public static int lose;

    public static void LoadCountfile()
    {
        if (File.Exists(playCountFile))
        {
            count = int.Parse(File.ReadAllText(playCountFile)) + 1;
            File.WriteAllText(playCountFile, String.Empty);
            File.AppendAllText(playCountFile, count.ToString());
        }
        else
        {
            _fileStream = new FileStream(playCountFile, FileMode.Create);
            _fileStream.Dispose();
            File.AppendAllText(playCountFile, count.ToString());
        }
    }

    private static string BuildSettingsString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Jumpforce: " + GameController.PlayerJumpForce + " ");
        sb.Append("Acceleration: " + GameController.PlayerAcceleration + " ");
        sb.Append("Airmovementmultiplier: " + GameController.PlayerAirMovementMultiplier + " ");
        sb.Append("Maxvelocity: " + GameController.PlayerMaxVelocity + " ");
        sb.Append("Gravitymultiplier: " + GravityController.GravityMultiplier + " ");
        sb.Append("CurrentControlSchemeIndex: " + GameController.CurrentControlSchemeIndex);
        return sb.ToString();
    }
    
    public static void WriteCompletionLog()
    {
        _completionLogLine = new CompletionLogLine(count, SceneManager.GetActiveScene().buildIndex, finishTime, gravityChanges, win, lose, BuildSettingsString());
        try
        {
            if (!File.Exists(logFile))
            {
                _fileStream = new FileStream(logFile, FileMode.Create);
                _fileStream.Dispose();
                File.AppendAllText(logFile, "sessionID;levelID;elapsedTime;GravityChanges;Win;Lose;Settings" + Environment.NewLine);
                File.AppendAllText(logFile, _completionLogLine.ToString() + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(logFile, _completionLogLine.ToString() + Environment.NewLine);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        } 
        win = 0;
        lose = 0;
    }
}

public class CompletionLogLine
{
    private readonly int _sessionID;
    private readonly int _levelID;
    private readonly float _elapsedTime;
    private readonly int _gravityChanges;
    private readonly int _win;
    private readonly int _lose;
    private readonly string _playerTestSettings;

    public CompletionLogLine(int count, int lID, float eTime, int gChanges, int w, int l, string pTestSettings)
    {
        _sessionID = count;
        _levelID = lID;
        _elapsedTime = eTime;
        _gravityChanges = gChanges;
        _win = w;
        _lose = l;
        _playerTestSettings = pTestSettings;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(_sessionID + ";");
        stringBuilder.Append(_levelID + ";");
        stringBuilder.Append(_elapsedTime + ";");
        stringBuilder.Append(_gravityChanges + ";");
        stringBuilder.Append(_win + ";");
        stringBuilder.Append(_lose + ";");
        stringBuilder.Append(_playerTestSettings);

        return stringBuilder.ToString();
    }
}