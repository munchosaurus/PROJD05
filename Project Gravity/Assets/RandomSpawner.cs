using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject FallingPrefab;

    // Update is called once per frame
    void Update()
    {
        Instantiate(FallingPrefab, randomSpawnPosition, Quaternion.identity);
    }
}
