using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] public LayerMask gravityChangeLayer;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float maxVelocity = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float decelleration = 3f;
    //[SerializeField] private Vector3 currentFacing; // moving to GravityController

    private InputAction.CallbackContext movementKeyInfo;
    private FormStates formStates;
    //private readonly float GRAVITY = 9.81f; // Moving to GravityController
    private Rigidbody playerRigidBody;
    //private bool isHorizontal = true; // Moving to gravityController
    private float JUMP_FORCE_MULTIPLIER = 100;
    private float jumpCooldownTimer = 0;

    // Crap below for level finishing
    private IngameMenu _menu;
    [SerializeField] private Transform levelTarget;

    // Start is called before the first frame update
    void Awake()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        playerRigidBody = GetComponent<Rigidbody>();
        _menu = gameObject.GetComponentInChildren<IngameMenu>();
        levelTarget = GameObject.FindWithTag("Target").gameObject.transform;
        formStates = gameObject.GetComponentInChildren<FormStates>();
        
        //currentFacing = new Vector3(0f, 1f, 0f); // moving to GravityController
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Will not perform anything if the lock state is true
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        MovePlayer();
        if (IsGoalReached())
        {
            if (!_menu.interactText.activeSelf)
            {
                _menu.interactText.SetActive(true);
            }
        }
        else
        {
            if (_menu.interactText.activeSelf)
            {
                _menu.interactText.SetActive(false);
            }
        }

        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, 0, eulerRotation.z);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void Interact()
    {
        if (IsGoalReached())
        {
            _menu.interactText.SetActive(false);
            _menu.Pause(1);
        }
    }

    private bool IsGoalReached()
    {
        return (Vector3.Distance(gameObject.transform.position, levelTarget.position) < 0.5f && IsGrounded());
    }

    // Used by input system
    public void SetMovementInput(InputAction.CallbackContext movement)
    {
        movementKeyInfo = movement;
    }

    private bool IsGrounded()
    {
        Vector3 boxCastDimensions = new Vector3(0.9f, 0.05f, 0.9f);

        return Physics.BoxCast(transform.position, boxCastDimensions, -transform.up, transform.rotation,
            transform.localScale.y / 2, groundLayer);
    }

    private void MovePlayer()
    {
        Debug.Log("Jag kör och " + movementKeyInfo.ReadValue<Vector2>().magnitude);
        if (IsGrounded() && formStates.GetCurrentForm().canMove)
        {
            Debug.Log("Kommer in i första ifsatsen");
            if (movementKeyInfo.ReadValue<Vector2>().magnitude == 0 && playerRigidBody.velocity.magnitude > 0)
            {
                playerRigidBody.AddForce(playerRigidBody.velocity.normalized * -decelleration);
            }
            else
            {
                Debug.Log("Kommer in i else");
                if (GravityController.IsGravityHorizontal())
                {
                    MoveHorizontal(movementKeyInfo.ReadValue<Vector2>().x);
                }
                else
                {
                    MoveVertical(movementKeyInfo.ReadValue<Vector2>().y);
                }
            }

            ClampMoveSpeed();
        }
    }

    private void MoveHorizontal(float direction)
    {
        if (ShouldAddMoreMoveForce(direction))
            playerRigidBody.AddForce(new Vector3(direction, 0, 0) * acceleration);
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
        if (playerRigidBody.velocity.magnitude > maxVelocity)
        {
            playerRigidBody.velocity = playerRigidBody.velocity.normalized * maxVelocity;
        }
    }

    // Used by input system.
    public void Jump()
    {
        if (jumpCooldownTimer <= 0 && formStates.GetCurrentForm().canMove && IsGrounded())
        {
            playerRigidBody.AddForce(transform.up * jumpForce * JUMP_FORCE_MULTIPLIER);
            jumpCooldownTimer = jumpCooldown;
        }
    }
    
    public void RotateToPlane()
    {
        if (formStates.GetCurrentForm().canMove)
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, GravityController.GetCurrentFacing());
        }
    }

    // Moved to GravityGun
    /*
    public void ShootGravityGun()
    {
        RaycastHit hit;
        Vector3 direction = GetMousePositionOnPlane() - transform.position;

        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, gravityChangeLayer,
                QueryTriggerInteraction.Collide) && hit.normal == hit.collider.transform.right)
        {
            currentFacing = hit.normal;
            Physics.gravity = -currentFacing * GRAVITY;
            RotateToPlane();
            isHorizontal = currentFacing.y != 0;
        }
    }*/

    // Moved to GravityGun
    /*
    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, Vector3.zero);
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }*/

    // moved to GravityController
    /*
    public Vector3 GetCurrentFacing()
    {
        return currentFacing;
    }

    public void SetCurrentFacing(Vector3 normal)
    {
        currentFacing = normal;
    }*/

}