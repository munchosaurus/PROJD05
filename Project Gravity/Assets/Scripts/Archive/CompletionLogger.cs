using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CompletionLogger
{
    private static readonly string LOGFile = Application.persistentDataPath + "/Logfile.csv";
    private static readonly string PlayCountFile = Application.persistentDataPath + "/Playcountfile.txt";
    private static CompletionLogLine _completionLogLine;
    private static FileStream _fileStream;
    private static int _count;
    private static float _finishTime;
    private static int _gravityChanges;
    private static int _win;
    private static int _lose;

    public static void LoadCountfile()
    {
        if (File.Exists(PlayCountFile))
        {
            _count = int.Parse(File.ReadAllText(PlayCountFile)) + 1;
            File.WriteAllText(PlayCountFile, String.Empty);
            File.AppendAllText(PlayCountFile, _count.ToString());
        }
        else
        {
            _fileStream = new FileStream(PlayCountFile, FileMode.Create);
            _fileStream.Dispose();
            File.AppendAllText(PlayCountFile, _count.ToString());
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
        _completionLogLine = new CompletionLogLine(_count, SceneManager.GetActiveScene().buildIndex, _finishTime, _gravityChanges, _win, _lose, BuildSettingsString());
        try
        {
            if (!File.Exists(LOGFile))
            {
                _fileStream = new FileStream(LOGFile, FileMode.Create);
                _fileStream.Dispose();
                File.AppendAllText(LOGFile, "sessionID;levelID;elapsedTime;GravityChanges;Win;Lose;Settings" + Environment.NewLine);
                File.AppendAllText(LOGFile, _completionLogLine.ToString() + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(LOGFile, _completionLogLine.ToString() + Environment.NewLine);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        } 
        _win = 0;
        _lose = 0;
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