using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private LayerMask gravityChangeLayer;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float maxVelocity = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float decelleration = 3f;

    private readonly float GRAVITY = 9.81f;
    private Rigidbody playerRigidBody;
    private bool isHorizontal = true;
    private float JUMP_FORCE_MULTIPLIER = 100;
    private float jumpCooldownTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootGravityGun();
        }

        if (IsGrounded())
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }

            float move;

            if (isHorizontal)
            {
                move = Input.GetAxis("Horizontal");
            }
            else
            {
                move = Input.GetAxis("Vertical");
            }

            MovePlayer(move);
        }

        if(jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private bool IsGrounded()
    {
        Vector3 boxCastDimensions = new Vector3(0.9f, 0.05f, 0.9f);

        return Physics.BoxCast(transform.position, boxCastDimensions, -transform.up, transform.rotation, transform.localScale.y / 2, groundLayer);
    }

    private void MovePlayer(float moveDirection)
    {
        if (moveDirection == 0 && playerRigidBody.velocity.magnitude > 0)
        {
            playerRigidBody.AddForce(playerRigidBody.velocity.normalized * -decelleration);
        }
        else {
            if (isHorizontal)
            {
                MoveHorizontal(moveDirection);
            }
            else
            {
                MoveVertical(moveDirection);
            }
        }
        ClampMoveSpeed();
    }

    private void MoveHorizontal(float direction)
    {
        if (ShouldAddMoreMoveForce(direction))
            playerRigidBody.AddForce(new Vector3 (direction, 0, 0) * acceleration);
    }

    private void MoveVertical(float direction)
    {
        if (ShouldAddMoreMoveForce(direction))
            playerRigidBody.AddForce(new Vector3(0, direction, 0) * acceleration);
    }

    private bool ShouldAddMoreMoveForce(float moveCoefficient)
    {
        return moveCoefficient != 0 && playerRigidBody.velocity.magnitude < maxVelocity;
    }

    private void ClampMoveSpeed()
    {
        if(playerRigidBody.velocity.magnitude > maxVelocity)
        {
            playerRigidBody.velocity = playerRigidBody.velocity.normalized * maxVelocity;
        }
    }

    private void Jump()
    {
        if (jumpCooldownTimer <= 0)
        {
            playerRigidBody.AddForce(transform.up * jumpForce * JUMP_FORCE_MULTIPLIER);
            jumpCooldownTimer = jumpCooldown;
        }
    }

    private void ShootGravityGun()
    {
        RaycastHit hit;
        Vector3 direction = GetMousePositionOnPlane() - transform.position;

        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, gravityChangeLayer, QueryTriggerInteraction.Collide) && hit.normal == hit.collider.transform.right)
        {
            Physics.gravity = -hit.normal * GRAVITY;
            transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
            isHorizontal = hit.normal.y != 0;
        }
    }

    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, Vector3.zero);
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, GetMousePositionOnPlane() - transform.position, Color.green);
    }
}
