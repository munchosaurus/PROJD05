using System;
using System.Collections.Generic;
using Mono.Collections.Generic;
using UnityEngine;

public static class LevelCompletionTracker
{
    public static List<int> unlockedLevels = new List<int>();
    public static Dictionary<int, float> levelRecords = new Dictionary<int, float>();

    public static void AddCompletedLevel(int levelIndex)
    {
        if (!unlockedLevels.Contains(levelIndex))
        {
            unlockedLevels.Add(levelIndex);
        }
    }

    public static void SetLevelBest(int levelID, float time)
    {
        if (!levelRecords.ContainsKey(levelID))
        {
            levelRecords.Add(levelID, time);
        }
        else if (levelRecords[levelID] > time)
        {
            levelRecords[levelID] = time;
        }
    }
}