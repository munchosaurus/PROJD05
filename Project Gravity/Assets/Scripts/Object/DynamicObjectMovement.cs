using System;
using System.Collections;
using UnityEngine;

public class DynamicObjectMovement : MonoBehaviour
{
    public Vector3 velocity;
    [SerializeField] private Vector3 horizontalCast, verticalCast;
    [SerializeField] bool groundedRight;
    [SerializeField] bool groundedLeft;
    [SerializeField] bool groundedUp;
    [SerializeField] bool groundedDown;
    [SerializeField] private float friction;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask magnetMask;

    private readonly float GRID_OFFSET = 0;
    private readonly float OBJECT_Z = 1;
    private Quaternion lockedRotation;
    private Vector3 boxCastDimensions;
    private bool isGrounded;
    public bool lockedToMagnet;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero;
        lockedRotation = new Quaternion(0, 0, 0, 0);
    }

    void FixedUpdate()
    {
        if (!lockedToMagnet)
        {
            velocity += Physics.gravity * Time.fixedDeltaTime;
            transform.rotation = lockedRotation;
        }

        CheckForCollisions();
        ApplyFriction();
        ApplyCollisions();

        if (lockedToMagnet)
        {
            return;
        }

        transform.position += velocity * Time.fixedDeltaTime;
    }

    public void MoveToMagnet(Vector3 location)
    {
        if (Vector3.Distance(transform.position, location) < 0.01f)
        {
            velocity = Vector3.zero;
        }
        else
        {
            transform.position =
                Vector3.MoveTowards(transform.position, location, velocity.magnitude / 2 * Time.fixedDeltaTime);
        }
    }

    private bool ShouldInheritMovement(GameObject otherObject, bool isHorizontal)
    {
        if (otherObject.GetComponentInParent<PlayerInput>() != null)
        {
            if (isHorizontal)
            {
                if (Math.Abs(otherObject.GetComponentInParent<PlayerInput>().velocity.x) > 0.1f)
                {
                    velocity.x = otherObject.GetComponentInParent<PlayerInput>().velocity.x;
                    return true;
                }
            }
            else
            {
                if (Math.Abs(otherObject.GetComponentInParent<PlayerInput>().velocity.y) > 0.1f)
                {
                    velocity.y = otherObject.GetComponentInParent<PlayerInput>().velocity.y;
                    return true;
                }
            }
        }
        else if (otherObject.GetComponent<DynamicObjectMovement>() != null)
        {
            if (isHorizontal)
            {
                if (Math.Abs(otherObject.GetComponent<DynamicObjectMovement>().velocity.x) < velocity.x &&
                    otherObject.GetComponent<DynamicObjectMovement>().velocity.x != 0)
                {
                    velocity.x = otherObject.GetComponent<DynamicObjectMovement>().velocity.x;
                    return true;
                }
            }
            else
            {
                if (Math.Abs(otherObject.GetComponent<DynamicObjectMovement>().velocity.y) < velocity.y &&
                    otherObject.GetComponent<DynamicObjectMovement>().velocity.y != 0)
                {
                    velocity.y = otherObject.GetComponent<DynamicObjectMovement>().velocity.y;
                    return true;
                }
            }
        }

        return false;
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
                    transform.localScale.y, groundMask))
            {
                ExtDebug.DrawBoxCastOnHit(transform.position, verticalCast, transform.rotation, Vector3.down,
                    hit.distance, Color.green);
                if (!ShouldInheritMovement(hit.collider.gameObject, false))
                {
                    groundedDown = true;
                    transform.position = new Vector3(transform.position.x,
                        GetClosestGridCentre(transform.position.y), transform.position.z);
                }
            }
        }
        else if (velocity.y > 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, transform.rotation,
                    transform.localScale.y, groundMask))
            {
                ExtDebug.DrawBoxCastOnHit(transform.position, verticalCast, transform.rotation, Vector3.up,
                    hit.distance, Color.green);
                if (!ShouldInheritMovement(hit.collider.gameObject, false))
                {
                    groundedUp = true;
                    transform.position = new Vector3(transform.position.x,
                        GetClosestGridCentre(transform.position.y), transform.position.z);
                }
            }
        }

        if (velocity.x > 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, transform.rotation,
                    transform.localScale.x, groundMask))
            {
                ExtDebug.DrawBoxCastOnHit(transform.position, horizontalCast, transform.rotation, Vector3.right,
                    hit.distance, Color.green);
                if (!ShouldInheritMovement(hit.collider.gameObject, true))
                {
                    groundedRight = true;
                    transform.position = new Vector3(
                        GetClosestGridCentre(transform.position.x),
                        transform.position.y, OBJECT_Z);
                }
            }
        }
        else if (velocity.x < 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, transform.rotation,
                    transform.localScale.x, groundMask))
            {
                ExtDebug.DrawBoxCastOnHit(transform.position, horizontalCast, transform.rotation, Vector3.left,
                    hit.distance, Color.green);
                if (!ShouldInheritMovement(hit.collider.gameObject, true))
                {
                    groundedLeft = true;
                    transform.position = new Vector3(
                        GetClosestGridCentre(transform.position.x),
                        transform.position.y, OBJECT_Z);
                }
            }
        }
    }

    private float GetClosestGridCentre(float origin)
    {
        if (Math.Abs(origin) > Math.Abs(Math.Round(origin)))
        {
            if (origin > 0)
            {
                return (float) Math.Round(Math.Abs(origin)) + GRID_OFFSET;
            }

            if (origin < 0)
            {
                return -((float) Math.Round(Math.Abs(origin)) + GRID_OFFSET);
            }
        }
        else
        {
            if (origin > 0)
            {
                return (float) Math.Round(Math.Abs(origin)) - GRID_OFFSET;
            }

            if (origin < 0)
            {
                return -((float) Math.Round(Math.Abs(origin)) - GRID_OFFSET);
            }
        }

        return origin;
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