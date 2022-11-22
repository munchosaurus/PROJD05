using UnityEngine;

[CreateAssetMenu(fileName = "New Level container", menuName = "ScriptableObjects/LevelContainer")]
public class LevelContainer : ScriptableObject
{
    public string levelName;
    public int levelID;
    public Sprite levelSprite;
    [TextArea] public string levelDescription;
}