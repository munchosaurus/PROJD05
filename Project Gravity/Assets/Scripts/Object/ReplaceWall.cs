using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

[ExecuteInEditMode]
public class ReplaceWall : MonoBehaviour
{
    public GameObject wallPrefab;
    public Vector3 dimensions;
    public bool isGround;
    public GameObject[] specialPrefabs;
    public Vector3[] specialPrefabPositions;
    public GameObject[] emblems;

    public Vector3[] emblemPositions;
    //public LayerMask[] specialLayerMasks;

    public List<GameObject> lavas;

    private void Awake()
    {
        List<GameObject> gos = new List<GameObject>();
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.hideFlags != HideFlags.None)
                continue;
            if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab ||
                PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
                continue;

            if (go.gameObject.name == "BuildingBlock_Hazard" && go.transform.position.z != 0)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(go);
                }
                    
            }
        }

        // foreach (var VARIABLE in gos)
        // {
        //     VARIABLE.font = fontToUse;
        // }
        Debug.Log(gos.Count + " är antalet hazards");
    }

    public void ReplaceLavas()
    {
    }
}