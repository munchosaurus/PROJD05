using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private LayerMask gravityChangeLayer;
    private readonly float GRAVITY = 9.81f;

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
            Vector3 direction = GetMousePositionOnPlane() - transform.position;

            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, gravityChangeLayer, QueryTriggerInteraction.Collide))
            {
                Physics.gravity = hit.normal * -1 * GRAVITY;
                Debug.Log("HIT - Normal: " + hit.normal);
                
            }
            Debug.Log("Gravity is now: " + Physics.gravity);
        }
    }

    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, Vector3.zero);
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, GetMousePositionOnPlane() - transform.position, Color.green);
    }
}
