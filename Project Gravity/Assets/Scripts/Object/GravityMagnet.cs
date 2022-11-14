using System;
using System.Collections;
using UnityEngine;

public class GravityMagnet : MonoBehaviour
{
    public bool triggered;
    [SerializeField] public bool upTriggered, downTriggered, leftTriggered, rightTriggered;
    [SerializeField] private Vector3 horizontalCast, verticalCast;
    [SerializeField] public DynamicObjectMovement dynamicObjectMovement;
    [SerializeField] public Vector3 detectionDirection;
    [SerializeField] private LayerMask gravityMagnet;
    [SerializeField] private float magnetRange;
    [SerializeField] private float magnetSpeed;
    private static Guid _gravityGunEventGuid;
    //private readonly float magnetCoolDown = 2f;

    

    private void Start()
    {
        EventSystem.Current.RegisterListener<GravityGunEvent>(ToggleMagnet, ref _gravityGunEventGuid);
    }

    private void FixedUpdate()
    {
        if (triggered)
        {
            CheckForBoxes();
            // if (dynamicObjectMovement != null)
            // {
            //     dynamicObjectMovement.SetMagnetPosition(transform.position + detectionDirection, magnetSpeed);
            // }
        }
    }

    private void ToggleMagnet(GravityGunEvent gravityGunEvent)
    {
        if (gravityGunEvent.TargetGameObject.layer == gameObject.layer)
        {
            if (gameObject.GetInstanceID() == gravityGunEvent.TargetGameObject.GetInstanceID())
            {
                if (dynamicObjectMovement == null)
                {
                    triggered = !triggered;
                }
                else
                {
                    dynamicObjectMovement.lockedToMagnet = false;
                    dynamicObjectMovement = null;
                    triggered = !triggered;
                }
            } else if (dynamicObjectMovement != null)
            {
                if (gravityGunEvent.TargetGameObject.GetInstanceID() == dynamicObjectMovement.gameObject.transform.GetChild(0).gameObject.GetInstanceID())
                {
                    dynamicObjectMovement.lockedToMagnet = false;
                    dynamicObjectMovement = null;
                    triggered = !triggered;
                }
            }
        }
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
                    detectionDirection = Vector3.down;
                    dynamicObjectMovement = hit.collider.GetComponentInParent<DynamicObjectMovement>();
                    dynamicObjectMovement.lockedToMagnet = true;
                    dynamicObjectMovement.SetMagnetPosition(transform.position + detectionDirection, magnetSpeed);
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
                    dynamicObjectMovement.SetMagnetPosition(transform.position + detectionDirection, magnetSpeed);
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
                    dynamicObjectMovement.SetMagnetPosition(transform.position + detectionDirection, magnetSpeed);
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
                    dynamicObjectMovement.SetMagnetPosition(transform.position + detectionDirection, magnetSpeed);
                    return;
                }
            }
        }
    }
}