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
    //private Guid _gravityGunEventGuid;
    private Quaternion lockedRotation;
    private Vector3 boxCastDimensions;
    private bool isGrounded;
    [SerializeField] private Vector3 horizontalCast, verticalCast;
    [SerializeField] bool groundedRight;
    [SerializeField] bool groundedLeft;
    [SerializeField] bool groundedUp;
    [SerializeField] bool groundedDown;
    [SerializeField] private float friction;


    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero;
        lockedRotation = new Quaternion(0, 0, 0, 0);

        //EventSystem.Current.RegisterListener<GravityGunEvent>(, ref _gravityGunEventGuid);
    }

    void FixedUpdate()
    {

        velocity += Physics.gravity * Time.fixedDeltaTime;
        transform.rotation = lockedRotation;
        
        CheckForCollisions();
        ApplyFriction();
        ApplyCollisions();
        
        transform.position += velocity * Time.fixedDeltaTime;
    }

    private void CheckForCollisions()
    {
        groundedDown = false;
        groundedUp = false;
        groundedLeft = false;
        groundedRight = false;
        RaycastHit hit;
        if (velocity.y < 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, transform.rotation,
                    transform.localScale.y / 2, groundMask))
            {
                ExtDebug.DrawBoxCastOnHit(transform.position, verticalCast, transform.rotation, Vector3.down,
                    hit.distance, Color.green);
                groundedDown = true;
                transform.position = new Vector3(transform.position.x,
                    hit.collider.transform.position.y + hit.collider.transform.localScale.y, transform.position.z);
            }
        }
        else if (velocity.y > 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, transform.rotation,
                    transform.localScale.y / 2, groundMask))
            {
                ExtDebug.DrawBoxCastOnHit(transform.position, verticalCast, transform.rotation, Vector3.up,
                    hit.distance, Color.green);
                groundedUp = true;
                transform.position = new Vector3(transform.position.x,
                    hit.collider.transform.position.y - hit.collider.transform.localScale.y, transform.position.z);
            }
        }

        if (velocity.x > 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, transform.rotation,
                    transform.localScale.x / 2, groundMask))
            {
                ExtDebug.DrawBoxCastOnHit(transform.position, horizontalCast, transform.rotation, Vector3.right,
                    hit.distance, Color.green);
                groundedRight = true;
                transform.position = new Vector3(
                    hit.collider.transform.position.x - hit.collider.transform.localScale.x,
                    transform.position.y, transform.position.z);
            }
        }
        else if (velocity.x < 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, transform.rotation,
                    transform.localScale.x / 2, groundMask))
            {
                ExtDebug.DrawBoxCastOnHit(transform.position, horizontalCast, transform.rotation, Vector3.left,
                    hit.distance, Color.green);
                groundedLeft = true;
                transform.position = new Vector3(
                    hit.collider.transform.position.x + hit.collider.transform.localScale.x,
                    transform.position.y, transform.position.z);
            }
        }
    }

    private void ApplyCollisions()
    {
        if ((groundedDown && velocity.y < 0) || (groundedUp && velocity.y > 0))
        {
            velocity.y = 0;
        }

        if ((groundedLeft && velocity.x < 0) || (groundedRight && velocity.x > 0))
        {
            velocity.x = 0;
        }
    }

    private void ApplyFriction()
    {
        if (groundedDown || groundedUp)
        {
            if (velocity.x > 0)
            {
                velocity.x -= friction * Time.fixedDeltaTime;
                if (velocity.x < 0)
                {
                    velocity.x = 0;
                }
            }
            else if (velocity.x < 0)
            {
                velocity.x += friction * Time.fixedDeltaTime;
                if (velocity.x > 0)
                {
                    velocity.x = 0;
                }
            }
        }

        if (groundedLeft || groundedRight)
        {
            if (velocity.y > 0)
            {
                velocity.y -= friction * Time.fixedDeltaTime;
                if (velocity.y < 0)
                {
                    velocity.y = 0;
                }
            }
            else if (velocity.y < 0)
            {
                velocity.y += friction * Time.fixedDeltaTime;
                if (velocity.y > 0)
                {
                    velocity.y = 0;
                }
            }
        }
    }
}