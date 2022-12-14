using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

[ExecuteInEditMode]
public class RemoveIncorrectWall : MonoBehaviour
{
    [SerializeField] List<String> namesToChange;

#if UNITY_EDITOR
    private void Awake()
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.hideFlags != HideFlags.None)
                continue;
            if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab ||
                PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
                continue;


            if (namesToChange.Contains(go.gameObject.name) && go.transform.position.z != 0)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(go);
                }
            }
        }
    }

#endif
}