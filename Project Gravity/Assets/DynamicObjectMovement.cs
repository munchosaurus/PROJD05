using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DynamicObjectMovement : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private LayerMask groundMask;
    private Guid _gravityGunEventGuid;
    private Quaternion lockedRotation;
    private Vector3 boxCastDimensions;
    private bool isGrounded;
    private Transform groundCheck;
    private Vector2 currentFacing;
    

    
    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero;
        groundCheck = gameObject.transform.GetChild(0).transform;
        lockedRotation = new Quaternion(0, 0, 0, 0);
        
        EventSystem.Current.RegisterListener<GravityGunEvent>(OnGravityChange, ref _gravityGunEventGuid);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = lockedRotation;
        RaycastHit hit;
        Physics.Raycast(transform.position, -GravityController.GetCurrentFacing(), out hit, 
            (transform.localScale.y / 2), groundMask);

        if (hit.collider)
        {
            Vector3 newPos = transform.position;
            currentFacing = GravityController.GetCurrentFacing();
            if (currentFacing.y < 0)
            {
                if (groundCheck.localPosition.y != 0.5f)
                {
                    groundCheck.localPosition = new Vector3(0,
                        0.5f, 0);
                }
                newPos.y -= Vector3.Distance(groundCheck.position, hit.point);
            } else if (currentFacing.y > 0)
            {
                if (groundCheck.localPosition.y != -0.5f)
                {
                    groundCheck.localPosition = new Vector3(0,
                        -0.5f, 0);
                }
                newPos.y += Vector3.Distance(groundCheck.position, hit.point);
            } else if (currentFacing.x < 0)
            {
                if (groundCheck.localPosition.x != 0.5f)
                {
                    groundCheck.localPosition = new Vector3(0.5f,
                        0, 0);
                }
                newPos.x -= Vector3.Distance(groundCheck.position, hit.point);
            }
            else
            {
                if (groundCheck.localPosition.x != -0.5f)
                {
                    groundCheck.localPosition = new Vector3(-0.5f,
                        0, 0);
                }
                newPos.x += Vector3.Distance(groundCheck.position, hit.point);
            }

            transform.position = newPos;
            velocity = Vector3.zero;
        }
        else
        {
            velocity += Physics.gravity * Time.fixedDeltaTime;
            transform.position += velocity * Time.fixedDeltaTime;
        }
    }

    private void OnGravityChange(GravityGunEvent gravityGunEvent)
    {
        currentFacing = gravityGunEvent.hitNormal;
    }
}
