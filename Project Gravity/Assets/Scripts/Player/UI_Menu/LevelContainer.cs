using UnityEngine;

[CreateAssetMenu(fileName = "New Level container", menuName = "ScriptableObjects/LevelContainer")]
public class LevelContainer : ScriptableObject
{
    public string levelName;
    public Sprite levelSprite;
    [TextArea] public string levelDescription;
    public float playLongClipTime;
    public AudioClip strictClip;
    public float strictClipTime;
}