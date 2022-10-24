using UnityEngine;

[CreateAssetMenu(fileName = "New Wall", menuName = "ScriptableObjects/Wall")]
public class Wall : ScriptableObject
{
    public GameObject wallPrefab;
    public Vector3 dimensions;
    public GameObject[] specialPrefabs;
    public Vector3[] specialPrefabPositions;
}
