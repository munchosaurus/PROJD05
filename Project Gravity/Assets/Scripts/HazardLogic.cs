using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardLogic : MonoBehaviour
{
    [SerializeField] private IngameMenu menu;
    [SerializeField] private Vector3 horizontalCast, verticalCast;
    [SerializeField] private LayerMask hazardMask;
    [SerializeField] private float collisionVelocityThreshold;
    private Rigidbody thisRigidBody;
    void Start()
    {
        menu = FindObjectOfType<IngameMenu>();
        thisRigidBody = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CheckForHazards();
    }

    
    private void CheckForHazards()
    {
        //Debug.Log(thisRigidBody.velocity.y);
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, transform.rotation,
                transform.localScale.y / 2, hazardMask))
        {
            if (menu != null && (thisRigidBody.velocity.y < collisionVelocityThreshold || Physics.gravity.y < 0))
            {
                // Game over
                menu.Pause(2);
            }
            else
            {
                Debug.Log("Cannot find menu script");
            }
        }

        if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, transform.rotation,
                transform.localScale.y / 2, hazardMask))
        {
            if (menu != null && (thisRigidBody.velocity.y > collisionVelocityThreshold || Physics.gravity.y > 0))
            {
                // Game over
                menu.Pause(2);
            }
            else
            {
                Debug.Log("Cannot find menu script");
            }
        }

        if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, transform.rotation,
                transform.localScale.x / 2, hazardMask))
        {
            if (menu != null  && (thisRigidBody.velocity.x > collisionVelocityThreshold || Physics.gravity.x > 0))
            {
                // Game over
                menu.Pause(2);
            }
            else
            {
                Debug.Log("Cannot find menu script");
            }
        }

        if (Physics.BoxCast(transform.position, verticalCast, Vector3.left, out hit, transform.rotation,
                transform.localScale.x / 2, hazardMask))
        {
            if (menu != null && (thisRigidBody.velocity.y < collisionVelocityThreshold || Physics.gravity.y < 0))
            {
                // Game over
                menu.Pause(2);
            }
            else
            {
                Debug.Log("Cannot find menu script");
            }
        }
    }
}
