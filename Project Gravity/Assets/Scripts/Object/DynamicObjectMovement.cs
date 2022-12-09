using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectMovement : MonoBehaviour
{
    public Vector3 velocity;

    [SerializeField] private Vector3 horizontalCast, verticalCast;

    // [SerializeField] bool groundedRight;
    // [SerializeField] bool groundedLeft;
    // [SerializeField] bool groundedUp;
    // [SerializeField] bool groundedDown;
    // [SerializeField] private float friction;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask magnetMask;
    [SerializeField] private GameObject effectHolder;

    public float collisionDefaultVolume;

    //private readonly float OBJECT_Z = 1;
    private Quaternion lockedRotation;
    private Vector3 boxCastDimensions;
    private bool isGrounded;
    public bool lockedToMagnet;
    private Vector3 magnetPosition;
    private const float ObjectCollisionGridClamp = 0.5f;

    private float magnetVel;

    // Start is called before the first frame update
    void Start()
    {
        collisionDefaultVolume = GetComponent<AudioSource>().volume;
        velocity = Vector3.zero;
        lockedRotation = new Quaternion(0, 0, 0, 0);
    }

    void FixedUpdate()
    {
        if (!lockedToMagnet)
        {
            if (effectHolder.activeSelf)
            {
                effectHolder.SetActive(false);
            }

            velocity += Physics.gravity * Time.fixedDeltaTime;
            transform.rotation = lockedRotation;
        }
        else
        {
            if (!effectHolder.activeSelf)
            {
                effectHolder.SetActive(true);
            }

            MoveToMagnet();
        }

        ApplyCollisions();
        ApplyFrictionStop();

        transform.position += velocity * Time.fixedDeltaTime;
    }

    public void SetMagnetPosition(Vector3 targetPos, float magnetSpeed)
    {
        magnetPosition = targetPos;
        magnetVel = magnetSpeed;
    }

    public void MoveToMagnet()
    {
        Vector3 nextPos = Vector3.MoveTowards(transform.position, magnetPosition, magnetVel * Time.fixedDeltaTime);
        if (nextPos == transform.position)
        {
            velocity = Vector3.zero;
        }
        else
        {
            float speedX = (Math.Abs(nextPos.x) - Math.Abs(transform.position.x)) * 50;
            float speedY = (Math.Abs(nextPos.y) - Math.Abs(transform.position.y)) * 50;

            velocity.x = speedX;
            velocity.y = speedY;
        }
    }

    // private bool ShouldInheritMovement(GameObject otherObject, bool isHorizontal)
    // {
    //     var otherMovement = new Vector3();
    //     if (otherObject.GetComponentInParent<PlayerController>() != null)
    //     {
    //         otherMovement = otherObject.GetComponentInParent<PlayerController>().velocity;
    //     }
    //     else if (otherObject.GetComponent<DynamicObjectMovement>() != null)
    //     {
    //         otherMovement = otherObject.GetComponent<DynamicObjectMovement>().velocity;
    //     }
    //
    //     if (otherMovement.magnitude > 0)
    //     {
    //         if (isHorizontal)
    //         {
    //             if (!(Math.Abs(otherMovement.x) < velocity.x) || otherMovement.x == 0) return false;
    //             velocity.x = otherMovement.x;
    //             return true;
    //         }
    //
    //         if (!(Math.Abs(otherMovement.y) < velocity.y) || otherMovement.y == 0) return false;
    //         velocity.y = otherMovement.y;
    //         return true;
    //     }
    //
    //     return false;
    // }

    private void CheckCollisionInMovement(Vector3 direction)
    {
        RaycastHit[] raycastHits;
        if (direction.y != 0 && Math.Abs(velocity.y) >
            Constants.COLLISION_SPEED_THRESHOLD * GameController.GlobalSpeedMultiplier)
        {
            raycastHits = Physics.BoxCastAll(transform.position, verticalCast, direction,
                Quaternion.identity,
                Mathf.Abs(transform.position.y - (transform.position + (velocity * Time.fixedDeltaTime)).y) +
                ObjectCollisionGridClamp, groundMask, QueryTriggerInteraction.UseGlobal);
            foreach (var collision in raycastHits)
            {
                if (collision.transform.gameObject.GetComponentInParent<PlayerController>())
                {
                    if (Math.Abs(collision.transform.gameObject.GetComponentInParent<PlayerController>().velocity.y) >
                        Constants.COLLISION_SPEED_THRESHOLD || lockedToMagnet)
                    {
                        return;
                    }
                }
                else if (collision.transform.gameObject.GetComponentInParent<DynamicObjectMovement>())
                {
                    if (Math.Abs(
                            collision.transform.gameObject.GetComponentInParent<DynamicObjectMovement>().velocity.y) >
                        Constants.COLLISION_SPEED_THRESHOLD &&
                        collision.transform.GetComponentInParent<DynamicObjectMovement>().GetInstanceID() !=
                        GetComponentInParent<DynamicObjectMovement>().GetInstanceID())
                    {
                        return;
                    }
                }
            }

            Event collisionEvent = new CollisionEvent()
            {
                SourceGameObject = gameObject,
            };
            EventSystem.Current.FireEvent(collisionEvent);
        }
        else if (direction.x != 0 && Math.Abs(velocity.x) >
                 Constants.COLLISION_SPEED_THRESHOLD * GameController.GlobalSpeedMultiplier)
        {
            raycastHits = Physics.BoxCastAll(transform.position, horizontalCast, direction,
                Quaternion.identity,
                Mathf.Abs(transform.position.x - (transform.position + (velocity * Time.fixedDeltaTime)).x) +
                ObjectCollisionGridClamp, groundMask, QueryTriggerInteraction.UseGlobal);
            foreach (var collision in raycastHits)
            {
                if (collision.transform.gameObject.GetComponentInParent<PlayerController>())
                {
                    if (Math.Abs(collision.transform.gameObject.GetComponentInParent<PlayerController>().velocity.x) >
                        Constants.COLLISION_SPEED_THRESHOLD || lockedToMagnet)
                    {
                        return;
                    }
                }
                else if (collision.transform.gameObject.GetComponentInParent<DynamicObjectMovement>())
                {
                    if (Math.Abs(
                            collision.transform.gameObject.GetComponentInParent<DynamicObjectMovement>().velocity.x) >
                        Constants.COLLISION_SPEED_THRESHOLD)
                    {
                        return;
                    }
                }
            }

            Event collisionEvent = new CollisionEvent()
            {
                SourceGameObject = gameObject,
            };
            EventSystem.Current.FireEvent(collisionEvent);
        }
    }

    /*
    * Dampens movement if needed, will check if the current velocity will place the player within a cube.
    * Also calls upon method handling collision sounds.
    */
    private void ApplyCollisions()
    {
        RaycastHit hit;
        Vector3 nextPos = transform.position + (velocity * Time.fixedDeltaTime);
        if (velocity.y < 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, Quaternion.identity,
                    Mathf.Abs(transform.position.y - nextPos.y) + ObjectCollisionGridClamp, groundMask))
            {
                if (Math.Abs(transform.position.y - nextPos.y) < Math.Abs(transform.position.y - hit.point.y))
                {
                    CheckCollisionInMovement(Vector3.down);
                    transform.position = new Vector3(transform.position.x, hit.point.y + ObjectCollisionGridClamp,
                        transform.position.z);
                    velocity.y = 0;
                }
            }
        }
        else if (velocity.y > 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, Quaternion.identity,
                    Mathf.Abs(transform.position.y - nextPos.y) + ObjectCollisionGridClamp, groundMask))
            {
                if (Math.Abs(transform.position.y - nextPos.y) < Math.Abs(transform.position.y - hit.point.y))
                {
                    CheckCollisionInMovement(Vector3.up);
                    transform.position = new Vector3(transform.position.x, hit.point.y - ObjectCollisionGridClamp,
                        transform.position.z);
                    velocity.y = 0;
                }
            }
        }

        if (velocity.x < 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, Quaternion.identity,
                    Mathf.Abs(transform.position.x - nextPos.x) + ObjectCollisionGridClamp, groundMask))
            {
                if (Math.Abs(transform.position.x - nextPos.x) < Math.Abs(transform.position.x - hit.point.x))
                {
                    CheckCollisionInMovement(Vector3.left);
                    transform.position = new Vector3(hit.point.x + ObjectCollisionGridClamp, transform.position.y,
                        transform.position.z);
                    velocity.x = 0;
                }
            }
        }
        else if (velocity.x > 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, Quaternion.identity,
                    Mathf.Abs(transform.position.x - nextPos.x) + ObjectCollisionGridClamp, groundMask))
            {
                if (Math.Abs(transform.position.x - nextPos.x) < Math.Abs(transform.position.x - hit.point.x))
                {
                    CheckCollisionInMovement(Vector3.right);
                    transform.position = new Vector3(hit.point.x - ObjectCollisionGridClamp, transform.position.y,
                        transform.position.z);
                    velocity.x = 0;
                }
            }
        }
    }

    private void ApplyFrictionStop()
    {
        if (Physics.gravity.x < 0 && velocity.y != 0)
        {
            if (IsGroundFoundInDirection(Vector3.left, horizontalCast))
            {
                if (ShouldObjectStopAfterCheckInDirection(Vector3.left, horizontalCast))
                {
                    velocity.y = 0;
                }
            }
        }
        else if (Physics.gravity.x > 0 && velocity.y != 0)
        {
            if (IsGroundFoundInDirection(Vector3.right, horizontalCast))
            {
                if (ShouldObjectStopAfterCheckInDirection(Vector3.right, horizontalCast))
                {
                    velocity.y = 0;
                }
            }
        }
        else if (Physics.gravity.y > 0 && velocity.x != 0)
        {
            if (IsGroundFoundInDirection(Vector3.up, verticalCast))
            {
                if (ShouldObjectStopAfterCheckInDirection(Vector3.up, verticalCast))
                {
                    velocity.x = 0;
                }
            }
        }
        else if (Physics.gravity.y < 0 && velocity.x != 0)
        {
            if (IsGroundFoundInDirection(Vector3.down, verticalCast))
            {
                if (ShouldObjectStopAfterCheckInDirection(Vector3.down, verticalCast))
                {
                    velocity.x = 0;
                }
            }
        }
    }

    private bool ShouldObjectStopAfterCheckInDirection(Vector3 direction, Vector3 boxcastDimensions)
    {
        List<RaycastHit> raycastHits = new List<RaycastHit>();
        foreach (var hit in Physics.BoxCastAll(transform.position, boxcastDimensions, direction,
                     Quaternion.identity,
                     transform.localScale.y / 2, groundMask, QueryTriggerInteraction.UseGlobal))
        {
            raycastHits.Add(hit);
        }

        foreach (var collision in raycastHits)
        {
            if (collision.transform.gameObject.GetComponentInParent<PlayerController>())
            {
                return false;
            }
            else if (collision.transform.gameObject.GetComponent<DynamicObjectMovement>())
            {
                if (collision.transform.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool IsGroundFoundInDirection(Vector3 direction, Vector3 boxcastDimensions)
    {
        if (Physics.BoxCast(transform.position, boxcastDimensions, direction, Quaternion.identity,
                transform.localScale.x / 2, groundMask))
        {
            return true;
        }

        return false;
    }
}
