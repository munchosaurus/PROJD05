using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallFunctions : MonoBehaviour
{
    [MenuItem("CONTEXT/Wall/Generate Wall")]
    static void GenerateWall(MenuCommand command)
    {
        Wall context = (Wall)command.context;
        GameObject parent = new GameObject("New Wall");

        if(context.specialPrefabs.Length > 0)
        {
            GenerateWallsWithSpecials(context, parent.transform);
        }
        else
        {
            GenerateWallWithoutSpecials(context, parent.transform);
        }
    }

    static void GenerateWallWithoutSpecials(Wall w, Transform parent)
    {
        for (int z = 0; z < w.dimensions.z; z++)
        {
            for (int x = 0; x < w.dimensions.x; x++)
            {
                for (int y = 0; y < w.dimensions.y; y++)
                {
                    GameObject go = Instantiate(w.wallPrefab, new Vector3(x, y, z), Quaternion.identity);

                    go.transform.parent = parent;
                }
            }
        }
    }

    static void GenerateWallsWithSpecials(Wall w, Transform parent)
    {
        for (int z = 0; z < w.dimensions.z; z++) {
            for (int x = 0; x < w.dimensions.x; x++)
            {
                for (int y = 0; y < w.dimensions.y; y++)
                {
                    Vector3 found = Array.Find(w.specialPrefabPositions, element => element.x == x && element.y == y);
                    GameObject go;

                    if (Array.Exists(w.specialPrefabPositions, element => element == found))
                    {
                        go = Instantiate(w.specialPrefabs[(int)found.z], new Vector3(x, y, z), Quaternion.identity);
                    }
                    else
                    {
                        go = Instantiate(w.wallPrefab, new Vector3(x, y, z), Quaternion.identity);
                    }

                    go.transform.parent = parent;
                }
            }
        }
    }
}
