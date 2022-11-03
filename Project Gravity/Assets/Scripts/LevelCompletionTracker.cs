using System;
using System.Collections.Generic;
public static class LevelCompletionTracker
{
    public static List<int> unlockedLevels = new List<int>();

    public static void AddCompletedLevel(int levelIndex)
    {
        if (!unlockedLevels.Contains(levelIndex))
        {
            unlockedLevels.Add(levelIndex);
        }
    }
}