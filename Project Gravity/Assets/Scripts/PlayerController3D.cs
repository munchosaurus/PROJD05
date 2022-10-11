using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    [SerializeField] private LayerMask gravityChangeLayer;
    private readonly float GRAVITY = 9.8f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, gravityChangeLayer))
            {
                Physics.gravity = hit.normal * -1 * GRAVITY;
            }
            Debug.Log("Gravity is now: " + Physics.gravity);
        }
    }
}
