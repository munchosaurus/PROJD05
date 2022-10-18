using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public LayerMask gravityChangeLayer;
    [SerializeField] private LayerMask groundLayer;
    private float _jumpForce;
    private float _jumpCooldown;
    private float _maxVelocity;
    private float _acceleration;
    private float _decelleration;
    private float _jumpForceMultiplier;
    private InputAction.CallbackContext _movementKeyInfo;
    private FormStates _formStates;
    private Rigidbody _playerRigidBody;
    private float _jumpCooldownTimer;
    private PlayerStats _playerStats;
    private IngameMenu _menu;
    [SerializeField] private Transform levelTarget;
    private Vector3 boxCastDimensions;

    // Start is called before the first frame update
    void Awake()
    {
        _playerStats = gameObject.GetComponent<PlayerStats>();
        _jumpForce = _playerStats.GetJumpForce();
        _jumpCooldown = _playerStats.GetJumpCooldown();
        _maxVelocity = _playerStats.GetMaxVelocity();
        _acceleration = _playerStats.GetPlayerMovementAcceleration();
        _decelleration = _playerStats.GetPlayerMovementDecelleration();
        _jumpForceMultiplier = _playerStats.GetJumpForceMultiplier();
        boxCastDimensions = new Vector3(0.9f, 0.05f, 0.9f);
        

        Physics.gravity = new Vector3(0, -9.81f, 0);
        _playerRigidBody = GetComponent<Rigidbody>();
        _menu = gameObject.GetComponentInChildren<IngameMenu>();
        levelTarget = GameObject.FindWithTag("Target").gameObject.transform;
        _formStates = gameObject.GetComponentInChildren<FormStates>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Will not perform anything if the lock state is true
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        if (_jumpCooldownTimer > 0)
        {
            _jumpCooldownTimer -= Time.deltaTime;
        }

        // Makes sure player is standing on ground and has a form that can move before calling upon method handling
        // user movement input

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
    
    private bool IsGoalReached()
    {
        return (Vector3.Distance(gameObject.transform.position, levelTarget.position) < 0.5f && IsGrounded());
    }

    private bool IsGrounded()
    {
        

        return Physics.BoxCast(transform.position, boxCastDimensions, -transform.up, transform.rotation,
            transform.localScale.y / 2, groundLayer);
    }

    // Called by input system
    public void Interact()
    {
        if (IsGoalReached())
        {
            _menu.interactText.SetActive(false);
            _menu.Pause(1);
        }
    }
    
    // Called by input system
    public void Jump()
    {
        if (_jumpCooldownTimer <= 0 && _formStates.GetCurrentForm().canMove && IsGrounded())
        {
            _playerRigidBody.AddForce(transform.up * _jumpForce * _jumpForceMultiplier);
            _jumpCooldownTimer = _jumpCooldown;
        }
    }
    
    // Called by input system
    public void SetMovementInput(InputAction.CallbackContext movement)
    {
        _movementKeyInfo = movement;
    }

    private void MovePlayer()
    {
        if (IsGrounded() && _formStates.GetCurrentForm().canMove)
        {
            if (_movementKeyInfo.ReadValue<Vector2>().magnitude == 0 && _playerRigidBody.velocity.magnitude > 0)
            {
                _playerRigidBody.AddForce(_playerRigidBody.velocity.normalized * -_decelleration);
            }
            else
            {
                if (GravityController.IsGravityHorizontal())
                {
                    MoveHorizontal(_movementKeyInfo.ReadValue<Vector2>().x);
                }
                else
                {
                    MoveVertical(_movementKeyInfo.ReadValue<Vector2>().y);
                }
            }
            ClampMoveSpeed();
        }
    }

    private void MoveHorizontal(float direction)
    {
        if (ShouldAddMoreMoveForce(direction))
            _playerRigidBody.AddForce(new Vector3(direction, 0, 0) * _acceleration);
    }

    private void MoveVertical(float direction)
    {
        if (ShouldAddMoreMoveForce(direction))
            _playerRigidBody.AddForce(new Vector3(0, direction, 0) * _acceleration);
    }

    private bool ShouldAddMoreMoveForce(float moveCoefficient)
    {
        return moveCoefficient != 0 && _playerRigidBody.velocity.magnitude < _maxVelocity;
    }

    private void ClampMoveSpeed()
    {
        if (_playerRigidBody.velocity.magnitude > _maxVelocity)
        {
            _playerRigidBody.velocity = _playerRigidBody.velocity.normalized * _maxVelocity;
        }
    }

    public void RotateToPlane()
    {
        if (_formStates.GetCurrentForm().canMove)
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, GravityController.GetCurrentFacing());
        }
    }
}