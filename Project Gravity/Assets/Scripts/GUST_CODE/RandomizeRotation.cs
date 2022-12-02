using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeRotation : MonoBehaviour
{
    void RandomizeMyRotation()
    {
        transform.rotation = Random.rotation;
    }
    void RandomizeYRotation()
    {
        Quaternion randYRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        transform.rotation = randYRotation;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        RandomizeMyRotation();
        //RandomizeYRotation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
