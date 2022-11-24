using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombine : MonoBehaviour
{
    private void Awake()
    {
        List<Material> materials = new();
        List<List<CombineInstance>> combineLists = new();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        foreach(MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.gameObject.GetComponent<MeshRenderer>();

            if (!meshRenderer || !meshFilter.sharedMesh || meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            // Divide meshes and submeshes by material
            for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
            {
                int materialIndex = materials.IndexOf(meshRenderer.sharedMaterials[i]);

                if (materialIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[i]);
                    combineLists.Add(new List<CombineInstance>());
                    materialIndex = materials.Count - 1;
                }

                CombineInstance combine = new();
                combine.transform = meshFilter.transform.localToWorldMatrix;
                combine.subMeshIndex = i;
                combine.mesh = meshFilter.sharedMesh;
                
                combineLists[materialIndex].Add(combine);
            }

            meshRenderer.enabled = false;
        }

        // Combine all meshes and submeshes for a specific material into one mesh
        // that will be a submesh in the final mesh

        Mesh[] newSubMeshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int i = 0; i < materials.Count; i++)
        {
            CombineInstance[] combineInstanceArray = combineLists[i].ToArray();
            newSubMeshes[i] = new Mesh();
            newSubMeshes[i].CombineMeshes(combineInstanceArray);

            combineInstances[i] = new CombineInstance();
            combineInstances[i].mesh = newSubMeshes[i];
            combineInstances[i].subMeshIndex = 0;
            combineInstances[i].transform = transform.worldToLocalMatrix;
        }

        // Combine all the material submeshes
        GetComponent<MeshFilter>().sharedMesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combineInstances, false, true);

        // Clear other meshes
        foreach (Mesh oldMesh in newSubMeshes)
        {
            oldMesh.Clear();
        }

        // Assign materials
        GetComponent<MeshRenderer>().materials = materials.ToArray();
    }
}
