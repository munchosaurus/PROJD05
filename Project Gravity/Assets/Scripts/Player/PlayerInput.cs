﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    public Vector3 velocity;

    [SerializeField] private Vector3 horizontalCast, verticalCast;
    [SerializeField] bool groundedRight;
    [SerializeField] bool groundedLeft;
    [SerializeField] bool groundedUp;
    [SerializeField] bool groundedDown;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask soundCollisionMask;

    private Vector3 _boxCastDimensions;
    private InputAction.CallbackContext _movementKeyInfo;

    [Header("Movement settings")] [SerializeField]
    private Vector3 _groundCheckDimensions;

    [SerializeField] private Vector3 _roofCheckDimensions;

    [SerializeField] private float _airMovementMultiplier;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _acceleration;
    private const float GRID_CLAMP_THRESHOLD = 0.02f;
    private float _jumpCooldownTimer;
    private AudioSource _audioSource;

    private readonly float OBJECT_Z = 1;


    // Start is called before the first frame update
    void Start()
    {
        GameController.SetInputLockState(true);
        Physics.gravity = new Vector3(0, -Constants.GRAVITY, 0);
        _audioSource = GetComponent<AudioSource>();
        StartCoroutine(SwitchInputLock());
        velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        if (_jumpCooldownTimer > 0)
        {
            _jumpCooldownTimer -= Time.deltaTime;
        }


        velocity += Physics.gravity * Time.fixedDeltaTime;
        MovePlayer();
        CheckForCollisions();
        ApplyCollisions();

        transform.position += velocity * Time.fixedDeltaTime;

        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, 0, eulerRotation.z);

        if (velocity.magnitude == 0)
        {
            ClampToGrid();
        }
    }

    /*
    * Turns the player input off for as many seconds as there are set in Constants.LEVEL_LOAD_INPUT_PAUSE_TIME
    */
    private IEnumerator SwitchInputLock()
    {
        yield return new WaitForSeconds(Constants.LEVEL_LOAD_INPUT_PAUSE_TIME);
        if (GameController.GetPlayerInputIsLocked())
        {
            GameController.SetInputLockState(false);
        }
        else
        {
            GameController.SetInputLockState(true);
        }
    }

    private void ClampToGrid()
    {
        Vector3 newPosition = transform.position;
        if (Math.Abs(gameObject.transform.position.x - Math.Round(gameObject.transform.position.x)) <
            GRID_CLAMP_THRESHOLD && !GravityController.IsGravityHorizontal())
        {
            newPosition.x = Mathf.Round(transform.position.x);
        }

        // if (Math.Abs(gameObject.transform.position.y - Math.Round(gameObject.transform.position.y)) <
        //     GRID_CLAMP_THRESHOLD && GravityController.IsGravityHorizontal())
        // {
        //     newPosition.y = Mathf.Round(transform.position.y);
        // }

        if (transform.position != newPosition)
        {
            transform.position = newPosition;
        }
    }

    public bool IsGrounded()
    {
        return Physics.BoxCast(transform.position, _groundCheckDimensions, -transform.up, transform.rotation,
            transform.localScale.y / 2, groundMask);
    }

    public bool IsRoofed()
    {
        return Physics.BoxCast(transform.position, _roofCheckDimensions, transform.up, transform.rotation,
            transform.localScale.y / 2, groundMask);
    }

    /*
     * Called by input system, adds _jumpForce to the velocity that is opposite to the
     * gravity in case the player is grounded and has passed the jump cool down.
     */
    public void Jump()
    {
        if (_jumpCooldownTimer <= 0 && IsGrounded() && !IsRoofed())
        {
            if (GravityController.IsGravityHorizontal())
            {
                if (Physics.gravity.x > 0 && !groundedLeft)
                {
                    velocity.x -= _jumpForce;
                }

                if (Physics.gravity.x < 0 && !groundedRight)
                {
                    velocity.x += _jumpForce;
                }
            }
            else
            {
                if (Physics.gravity.y > 0 && !groundedDown)
                {
                    velocity.y -= _jumpForce;
                }

                if (Physics.gravity.y < 0 && !groundedUp)
                {
                    velocity.y += _jumpForce;
                }
            }

            _jumpCooldownTimer = _jumpCooldown;
        }
    }

/*
 * Sets the movement float, will be read by MovePlayer
 */
    public void SetMovementInput(InputAction.CallbackContext movement)
    {
        _movementKeyInfo = movement;
    }

    private bool ShouldInheritMovement(GameObject otherObject, bool isHorizontal)
    {
        if (otherObject.GetComponent<DynamicObjectMovement>() == null) return false;
        var dynamicObjectMovement = otherObject.GetComponent<DynamicObjectMovement>();
        if (isHorizontal)
        {
            if (!(Math.Abs(dynamicObjectMovement.velocity.x) < Math.Abs(velocity.x)) ||
                dynamicObjectMovement.velocity.x == 0) return false;
            velocity.x = dynamicObjectMovement.velocity.x;
            return true;
        }

        if (!(Math.Abs(dynamicObjectMovement.velocity.y) < Math.Abs(velocity.y)) ||
            dynamicObjectMovement.velocity.y == 0) return false;
        velocity.y = dynamicObjectMovement.velocity.y;
        return true;
    }

    private void CheckCollisionInMovement(Vector3 direction)
    {
        List<int> layers = new List<int>();
        RaycastHit[] raycastHits = new RaycastHit[0];
        if (direction.y != 0 && Math.Abs(velocity.y) > Constants.COLLISION_SPEED_THRESHOLD)
        {
            raycastHits = Physics.BoxCastAll(transform.position, verticalCast, direction,
                Quaternion.identity,
                transform.localScale.y / 2, soundCollisionMask, QueryTriggerInteraction.UseGlobal);
            foreach (var collision in raycastHits)
            {
                if (collision.transform.GetComponentInParent<DynamicObjectMovement>())
                {
                    if (collision.transform.GetComponentInParent<DynamicObjectMovement>().velocity.magnitude < Constants.COLLISION_SPEED_THRESHOLD)
                    {
                        layers.Add(collision.collider.gameObject.layer);
                    }
                }
                else
                {
                    layers.Add(collision.collider.gameObject.layer);
                }
            }
        }
        else if (direction.x != 0 && Math.Abs(velocity.x) > Constants.COLLISION_SPEED_THRESHOLD)
        {
            raycastHits = Physics.BoxCastAll(transform.position, horizontalCast, direction,
                Quaternion.identity,
                transform.localScale.y / 2, soundCollisionMask, QueryTriggerInteraction.UseGlobal);
            foreach (var collision in raycastHits)
            {
                if (collision.transform.GetComponentInParent<DynamicObjectMovement>())
                {
                    if (collision.transform.GetComponentInParent<DynamicObjectMovement>().velocity.magnitude < Constants.COLLISION_SPEED_THRESHOLD)
                    {
                        layers.Add(collision.collider.gameObject.layer);
                    }
                }
                else
                {
                    layers.Add(collision.collider.gameObject.layer);
                }
            }
        }

        if (layers.Count > 0)
        {
            Event collisionEvent = new CollisionEvent()
            {
                SourceGameObject = gameObject,
                Layers = layers
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
                if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, Quaternion.identity,
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
                if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, Quaternion.identity,
                        transform.localScale.y / 2, groundMask))
                {
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
                if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, Quaternion.identity,
                        transform.localScale.x / 2, groundMask))
                {
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
                if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, Quaternion.identity,
                        transform.localScale.x / 2, groundMask))
                {
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

    private void MovePlayer()
    {
        if (!IsGrounded())
        {
            if (GravityController.IsGravityHorizontal())
            {
                MoveVertical(_movementKeyInfo.ReadValue<Vector2>().y * _airMovementMultiplier);
            }
            else
            {
                MoveHorizontal(_movementKeyInfo.ReadValue<Vector2>().x * _airMovementMultiplier);
            }
        }
        else
        {
            if (GravityController.IsGravityHorizontal())
            {
                MoveVertical(_movementKeyInfo.ReadValue<Vector2>().y);
            }
            else
            {
                MoveHorizontal(_movementKeyInfo.ReadValue<Vector2>().x);
            }
        }
    }

    private void MoveHorizontal(float direction)
    {
        if (IsGrounded())
        {
            if (direction == 0 || (velocity.x > 0 && direction < 0) || (velocity.x < 0 && direction > 0))
            {
                _audioSource.mute = true;
                velocity.x = 0;
            }
            else
            {
                _audioSource.mute = false;
            }
        }
        else
        {
            _audioSource.mute = true;
        }

        if (ShouldAddMoreMoveForce(direction))
            velocity.x += direction * _acceleration * Time.fixedDeltaTime;
    }

    private void MoveVertical(float direction)
    {
        if (IsGrounded())
        {
            if (direction == 0 || (velocity.y > 0 && direction < 0) || (velocity.y < 0 && direction > 0))
            {
                _audioSource.mute = true;
                velocity.y = 0;
            }
            else
            {
                _audioSource.mute = false;
            }
        }
        else
        {
            _audioSource.mute = true;
        }

        if (ShouldAddMoreMoveForce(direction))
            velocity.y += direction * _acceleration * Time.fixedDeltaTime;
    }

    private bool ShouldAddMoreMoveForce(float moveCoefficient)
    {
        var dir = new Vector3();
        var magnitude = velocity.x + moveCoefficient;
        if (GravityController.IsGravityHorizontal())
        {
            magnitude = velocity.y + moveCoefficient;
            _boxCastDimensions = new Vector3(0.47f, 0.01f, 0.47f);
            if (moveCoefficient > 0)
            {
                dir = Vector3.up;
            }
            else if (moveCoefficient < 0)
            {
                dir = Vector3.down;
            }
        }
        else
        {
            _boxCastDimensions = new Vector3(0.01f, 0.47f, 0.47f);
            if (moveCoefficient > 0)
            {
                dir = Vector3.right;
            }
            else if (moveCoefficient < 0)
            {
                dir = Vector3.left;
            }
        }

        if (moveCoefficient != 0)
        {
            return Math.Abs(magnitude) < _maxVelocity && !BoxCast(dir);
        }

        return moveCoefficient != 0 && Math.Abs(magnitude) < _maxVelocity;
    }

    private bool BoxCast(Vector3 direction)
    {
        RaycastHit hit;
        Physics.BoxCast(transform.position, _boxCastDimensions, direction, out hit, Quaternion.identity,
            transform.localScale.y / 2, groundMask);
        ExtDebug.DrawBoxCastOnHit(transform.position, _boxCastDimensions, Quaternion.identity, direction,
            hit.distance, Color.red);

        return hit.collider;
    }

    public void RotateToPlane()
    {
        transform.rotation = Quaternion.LookRotation(transform.forward, -GravityController.GetCurrentFacing());
    }
}