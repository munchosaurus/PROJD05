using System;
using System.Collections;
using System.Collections.Generic;
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

    public void MoveToMagnet(Vector3 location, float magnetSpeed)
    {
        if (Vector3.Distance(transform.position, location) < 0.01f)
        {
            velocity = Vector3.zero;
        }
        else
        {
            if (velocity.magnitude < 0.01f)
            {
                velocity.x += 0.03f;
            }
            else
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, location, magnetSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private bool ShouldInheritMovement(GameObject otherObject, bool isHorizontal)
    {
        var otherMovement = new Vector3();
        if (otherObject.GetComponentInParent<PlayerInput>() != null)
        {
            otherMovement = otherObject.GetComponentInParent<PlayerInput>().velocity;
        }
        else if (otherObject.GetComponent<DynamicObjectMovement>() != null)
        {
            otherMovement = otherObject.GetComponent<DynamicObjectMovement>().velocity;
        }

        if (otherMovement.magnitude > 0)
        {
            if (isHorizontal)
            {
                if (!(Math.Abs(otherMovement.x) < velocity.x) || otherMovement.x == 0) return false;
                velocity.x = otherMovement.x;
                return true;
            }

            if (!(Math.Abs(otherMovement.y) < velocity.y) || otherMovement.y == 0) return false;
            velocity.y = otherMovement.y;
            return true;
        }

        return false;
    }

    private void CheckCollisionInMovement(Vector3 direction)
    {
        RaycastHit[] raycastHits;
        if (direction.y != 0 && Math.Abs(velocity.y) > Constants.GRAVITY * Time.fixedDeltaTime)
        {
            raycastHits = Physics.BoxCastAll(transform.position, verticalCast, direction,
                Quaternion.identity,
                transform.localScale.y / 2, groundMask, QueryTriggerInteraction.UseGlobal);
            foreach (var collision in raycastHits)
            {
                if (collision.transform.gameObject.GetComponentInParent<PlayerInput>())
                {
                    return;
                }
            }

            Event collisionEvent = new CollisionEvent()
            {
                SourceGameObject = gameObject,
            };
            EventSystem.Current.FireEvent(collisionEvent);
        }
        else if (direction.x != 0 && Math.Abs(velocity.x) > Constants.GRAVITY * Time.fixedDeltaTime)
        {
            raycastHits = Physics.BoxCastAll(transform.position, horizontalCast, direction,
                Quaternion.identity,
                transform.localScale.y / 2, groundMask, QueryTriggerInteraction.UseGlobal);
            foreach (var collision in raycastHits)
            {
                if (collision.transform.gameObject.GetComponentInParent<PlayerInput>())
                {
                    return;
                }
            }

            Event collisionEvent = new CollisionEvent()
            {
                SourceGameObject = gameObject,
            };
            EventSystem.Current.FireEvent(collisionEvent);
        }
    }


    private void CheckForCollisions()
    {
        groundedDown = false;
        groundedUp = false;
        groundedLeft = false;
        groundedRight = false;
        RaycastHit hit;
        switch (velocity.y)
        {
            case < 0:
            {
                if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, transform.rotation,
                        transform.localScale.y / 2, groundMask))
                {
                    if (!ShouldInheritMovement(hit.collider.gameObject, false))
                    {
                        groundedDown = true;
                        transform.position = new Vector3(transform.position.x,
                            GetClosestGridCentre(transform.position.y), transform.position.z);
                    }

                    CheckCollisionInMovement(Vector3.down);
                }

                break;
            }
            case > 0:
            {
                if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, transform.rotation,
                        transform.localScale.y / 2, groundMask))
                {
                    ExtDebug.DrawBoxCastOnHit(transform.position, verticalCast, transform.rotation, Vector3.up,
                        hit.distance, Color.green);
                    if (!ShouldInheritMovement(hit.collider.gameObject, false))
                    {
                        groundedUp = true;
                        transform.position = new Vector3(transform.position.x,
                            GetClosestGridCentre(transform.position.y), transform.position.z);
                    }

                    CheckCollisionInMovement(Vector3.up);
                }

                break;
            }
        }

        switch (velocity.x)
        {
            case > 0:
            {
                if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, transform.rotation,
                        transform.localScale.x / 2, groundMask))
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

                    CheckCollisionInMovement(Vector3.right);
                }

                break;
            }
            case < 0:
            {
                if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, transform.rotation,
                        transform.localScale.x / 2, groundMask))
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

                    CheckCollisionInMovement(Vector3.left);
                }

                break;
            }
        }
    }

    private float GetClosestGridCentre(float origin)
    {
        if (Math.Abs(origin) > Math.Abs(Math.Round(origin)))
        {
            if (origin > 0)
            {
                return (float) Math.Round(Math.Abs(origin));
            }

            if (origin < 0)
            {
                return -((float) Math.Round(Math.Abs(origin)));
            }
        }
        else
        {
            if (origin > 0)
            {
                return (float) Math.Round(Math.Abs(origin));
            }

            if (origin < 0)
            {
                return -((float) Math.Round(Math.Abs(origin)));
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