using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
    [SerializeField] private Vector3 horizontalCast, verticalCast;
    bool groundedRight;
    bool groundedLeft;
    bool groundedUp;
    bool groundedDown;


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
        velocity += Physics.gravity * Time.fixedDeltaTime;
        transform.rotation = lockedRotation;

        RaycastHit hit;

        if (velocity.y < 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, transform.rotation,
                    transform.localScale.y / 2, groundMask))
            {
                groundedDown = true;
                transform.position = new Vector3(transform.position.x,
                    hit.collider.transform.position.y + hit.collider.transform.localScale.y, transform.position.z);
            }
            else
            {
                groundedDown = false;
            }
        } else if (velocity.y > 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, transform.rotation,
                    transform.localScale.y / 2, groundMask))
            {
                groundedUp = true;
                transform.position = new Vector3(transform.position.x,
                    hit.collider.transform.position.y - hit.collider.transform.localScale.y, transform.position.z);
            }
            else
            {
                groundedUp = false;
            }
        }



        if (velocity.x > 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, transform.rotation,
                    transform.localScale.x / 2, groundMask))
            {
                groundedRight = true;
                transform.position = new Vector3(hit.collider.transform.position.x - hit.collider.transform.localScale.x,
                    transform.position.y, transform.position.z);
            }
            else
            {
                groundedRight = false;
            }
        } else if (velocity.x < 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, transform.rotation,
                    transform.localScale.x / 2, groundMask))
            {
                groundedLeft = true;
                transform.position = new Vector3(hit.collider.transform.position.x + hit.collider.transform.localScale.x,
                    transform.position.y, transform.position.z);
            }
            else
            {
                groundedLeft = false;
            }
        }





        if (groundedDown && Physics.gravity.y < 0)
        {
            velocity.y = 0;
        } else if (groundedUp && Physics.gravity.y > 0)
        {
            velocity.y = 0;
        }

        if (groundedLeft && Physics.gravity.x < 0)
        {
            velocity.x = 0;
        } else if (groundedRight && Physics.gravity.x > 0)
        {
            velocity.x = 0;
        }

        transform.position += velocity * Time.fixedDeltaTime;

        // RaycastHit hit;
        // Physics.Raycast(transform.position, -GravityController.GetCurrentFacing(), out hit, 
        //     (transform.localScale.y / 2), groundMask);
        //
        // if (hit.collider)
        // {
        //     Vector3 newPos = transform.position;
        //     currentFacing = GravityController.GetCurrentFacing();
        //     if (currentFacing.y < 0)
        //     {
        //         if (groundCheck.localPosition.y != 0.5f)
        //         {
        //             groundCheck.localPosition = new Vector3(0,
        //                 0.5f, 0);
        //         }
        //         newPos.y -= Vector3.Distance(groundCheck.position, hit.point);
        //     } else if (currentFacing.y > 0)
        //     {
        //         if (groundCheck.localPosition.y != -0.5f)
        //         {
        //             groundCheck.localPosition = new Vector3(0,
        //                 -0.5f, 0);
        //         }
        //         newPos.y += Vector3.Distance(groundCheck.position, hit.point);
        //     } else if (currentFacing.x < 0)
        //     {
        //         if (groundCheck.localPosition.x != 0.5f)
        //         {
        //             groundCheck.localPosition = new Vector3(0.5f,
        //                 0, 0);
        //         }
        //         newPos.x -= Vector3.Distance(groundCheck.position, hit.point);
        //     }
        //     else
        //     {
        //         if (groundCheck.localPosition.x != -0.5f)
        //         {
        //             groundCheck.localPosition = new Vector3(-0.5f,
        //                 0, 0);
        //         }
        //         newPos.x += Vector3.Distance(groundCheck.position, hit.point);
        //     }
        //
        //     transform.position = newPos;
        //     velocity = Vector3.zero;
        // }
        // else
        // {
        //     velocity += Physics.gravity * Time.fixedDeltaTime;
        //     transform.position += velocity * Time.fixedDeltaTime;
        // }
    }

    private void OnGravityChange(GravityGunEvent gravityGunEvent)
    {
        currentFacing = gravityGunEvent.hitNormal;
    }
}