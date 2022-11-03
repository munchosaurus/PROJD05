using System;
using UnityEngine;

public class GravityMagnet : MonoBehaviour
{
    [SerializeField] private bool triggered;
    [SerializeField] public bool upTriggered, downTriggered, leftTriggered, rightTriggered;
    [SerializeField] private Vector3 horizontalCast, verticalCast;
    [SerializeField] public DynamicObjectMovement dynamicObjectMovement;
    [SerializeField] public Vector3 detectionDirection;
    [SerializeField] private LayerMask gravityMagnet;
    [SerializeField] private float magnetRange;


    private void FixedUpdate()
    {
        if (triggered)
        {
            CheckForBoxes();
            if (dynamicObjectMovement != null)
            {
                dynamicObjectMovement.MoveToMagnet(transform.position + detectionDirection);
            }
        }
    }

    private void ToggleMagnet()
    {
        dynamicObjectMovement.lockedToMagnet = false;
        dynamicObjectMovement = null;
        triggered = !triggered;
    }
    
    private void CheckForBoxes()
    {
        RaycastHit hit;
        if (downTriggered && dynamicObjectMovement == null)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, Quaternion.identity,
                    magnetRange, gravityMagnet, QueryTriggerInteraction.Collide))
            {
                
                if (hit.collider.GetComponentInParent<DynamicObjectMovement>() != null)
                {
                    Debug.Log("Found it");
                    detectionDirection = Vector3.down;
                    dynamicObjectMovement = hit.collider.GetComponentInParent<DynamicObjectMovement>();
                    dynamicObjectMovement.lockedToMagnet = true;
                    return;
                }
            }
            
        }
        
        if (upTriggered && dynamicObjectMovement == null)
        {

            if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, Quaternion.identity,
                    magnetRange, gravityMagnet, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.GetComponentInParent<DynamicObjectMovement>() != null)
                {
                    detectionDirection = Vector3.up;
                    dynamicObjectMovement = hit.collider.GetComponentInParent<DynamicObjectMovement>();
                    dynamicObjectMovement.lockedToMagnet = true;
                    return;
                }
            }
        }
        
        if (rightTriggered && dynamicObjectMovement == null)
        {

            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, Quaternion.identity,
                    magnetRange, gravityMagnet, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.GetComponentInParent<DynamicObjectMovement>() != null)
                {
                    detectionDirection = Vector3.right;
                    dynamicObjectMovement = hit.collider.GetComponentInParent<DynamicObjectMovement>();
                    dynamicObjectMovement.lockedToMagnet = true;
                    return;
                }
            }
        }

        ExtDebug.DrawBoxCastBox(transform.position, horizontalCast, Quaternion.identity, Vector3.left,
            transform.localScale.x / 2, Color.red);
        if (leftTriggered && dynamicObjectMovement == null)
        {

            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, Quaternion.identity,
                    magnetRange, gravityMagnet, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.GetComponentInParent<DynamicObjectMovement>() != null)
                {
                    detectionDirection = Vector3.left;
                    dynamicObjectMovement = hit.collider.GetComponentInParent<DynamicObjectMovement>();
                    dynamicObjectMovement.lockedToMagnet = true;
                    return;
                }
            }
        }
    }
}