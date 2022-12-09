using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    // [SerializeField] private LayerMask gravityChangeLayer;
    // private readonly float GRAVITY = 9.8f;
    // public new Transform camera;
    // public float speed = 6f;
    // // Keep in mind that that the vectors are the opposite value to how they are added
    // public float turnSmoothTime = 0.05f;
    // private float _turnSmoothVelocity;

    // Start is called before the first frame update
    // void Start()
    // {
    //     camera = GameObject.FindWithTag("MainCamera").transform;
    // }

    // Update is called once per frame
    void Update()
    {
        // // Handles gravity changes
        // if (Input.GetKeyDown(KeyCode.Mouse0))
        // {
        //     RaycastHit hit;
        //     if(Physics.Raycast(transform.position, camera.transform.forward, out hit, Mathf.Infinity, gravityChangeLayer))
        //     {
        //         Physics.gravity = hit.normal * -1 * GRAVITY;
        //     }
        //     
        // }

        // Handles movement and aim
        // float horizontal = Input.GetAxisRaw("Horizontal");
        // float vertical = Input.GetAxisRaw("Vertical");
        // Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        // if (direction.magnitude >= 0.1f)
        // {
        //     float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
        //     float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
        //         turnSmoothTime);
        //     transform.rotation = Quaternion.Euler(0f, angle, 0f);
        //
        //     Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        //     transform.position += (moveDirection.normalized * speed * Time.deltaTime);
        // }
    }
}
