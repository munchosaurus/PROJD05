using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public static class LevelCompletionTracker
{
    public static List<int> unlockedLevels = new List<int>();
    public static Dictionary<int, float> levelRecords = new Dictionary<int, float>();

    public static void AddUnlockedLevel(int levelIndex)
    {
        if (!unlockedLevels.Contains(levelIndex) && levelIndex <= SceneManager.sceneCountInBuildSettings - 1)
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

    public static bool IsTimeNewRecord(int levelID, float time)
    {
        if (levelRecords[levelID] < time) return false;
        return true;
    }

    public static bool LevelHasRecord(int levelID)
    {
        return levelRecords.Keys.Contains(levelID);
    }

    public static void LoadLevels(LevelData levelData)
    {
        if (levelData.LevelRecords.Count > 0)
        {
            levelRecords = levelData.LevelRecords;

            int max = levelRecords.Keys.Max();

            for (int i = 0; i <= max; i++)
            {
                AddUnlockedLevel(i + 1);
            }
        }
    }
}