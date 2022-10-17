using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MeshTransparencyHandler : MonoBehaviour
{
    [SerializeField] private GameObject camera;
    [SerializeField] private CinemachineFreeLook cameraSettings;
    private GameObject graphicsTransform;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        graphicsTransform = gameObject.transform.GetChild(0).gameObject;
        cameraSettings = camera.GetComponent<CinemachineFreeLook>();

    }

    void Update()
    {
        if (Vector3.Distance(graphicsTransform.transform.position, camera.transform.position) < cameraSettings.m_Orbits[1].m_Radius)
        {
            float alpha = Vector3.Distance(graphicsTransform.transform.position, camera.transform.position) /
                          cameraSettings.m_Orbits[1].m_Radius;
            graphicsTransform.GetComponent<MeshRenderer>().material.color = new Color(0f,0f,1f,alpha) ;
        }
        else
        {
            graphicsTransform.GetComponent<MeshRenderer>().material.color = new Color(0f,0f,1f,1) ;
        }
    }
}
