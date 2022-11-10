using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallFunctions : MonoBehaviour
{
    private const float EMBLEM_Z_POS = -0.6f;

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

        if (context.isGround)
        {
            SetupBoxCollider(context, parent);

            Debug.Log(LayerMask.NameToLayer("Ground"));
            parent.layer = LayerMask.NameToLayer("Ground");
        }

        if(context.emblems.Length > 0)
        {
            AddEmblems(context, parent.transform);
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
                    GameObject go = PrefabUtility.InstantiatePrefab(w.wallPrefab) as GameObject;

                    go.transform.position = new Vector3(x, y, z);
                    go.transform.rotation = Quaternion.identity;
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
                    GameObject go;

                    if (IsSpecial(w, x, y))
                    {
                        Vector3 found = Array.Find(w.specialPrefabPositions, element => element.x == x && element.y == y);
                        go = PrefabUtility.InstantiatePrefab(w.specialPrefabs[(int)found.z]) as GameObject;
                    }
                    else
                    {
                        
                        go = PrefabUtility.InstantiatePrefab(w.wallPrefab) as GameObject;
                    }

                    go.transform.position = new Vector3(x, y, z);
                    go.transform.rotation = Quaternion.identity;
                    go.transform.parent = parent;
                }
            }
        }
    }

    static bool IsSpecial(Wall w, int x, int y)
    {
        bool isSpecial = false;

        foreach(Vector3 v in w.specialPrefabPositions)
        {
            if(v.x == x && v.y == y)
            {
                isSpecial = true;
            }
        }

        return isSpecial;
    }

    static void SetupBoxCollider(Wall context, GameObject parent)
    {
        BoxCollider boxCol = parent.AddComponent<BoxCollider>();
        float boxColXpos = context.dimensions.x / 2 - 0.5f;
        float boxColYpos = context.dimensions.y / 2 - 0.5f;
        float boxColZpos = context.dimensions.z / 2 - 0.5f;
        boxCol.center = new Vector3(boxColXpos, boxColYpos, boxColZpos);
        boxCol.size = new Vector3(context.dimensions.x, context.dimensions.y, context.dimensions.z);
    }

    static void AddEmblems(Wall w, Transform parent)
    {
        GameObject go;
        foreach(Vector3 v in w.emblemPositions)
        {
            go = PrefabUtility.InstantiatePrefab(w.emblems[(int)v.z]) as GameObject;
            go.transform.position = new Vector3(v.x, v.y, EMBLEM_Z_POS);
            go.transform.parent = parent;
        }
    }
}
