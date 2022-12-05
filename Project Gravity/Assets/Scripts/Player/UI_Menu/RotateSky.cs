using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky : MonoBehaviour {

    public float RotateSpeed = 1.2f;
    
    void Update() {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
    }
}

//Tutorial for Reference:
//https://youtu.be/cqGq__JjhMM