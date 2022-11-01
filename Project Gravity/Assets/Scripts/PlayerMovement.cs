using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform levelTarget;
    [SerializeField] private LayerMask groundMask;
    private InputAction.CallbackContext _movementKeyInfo;
    private Rigidbody _playerRigidBody;
    private PlayerStats _playerStats;
    private IngameMenu _menu;
    private Vector3 groundCheckDimensions;
    private Vector3 boxCastDimensions;
    private float _jumpCooldownTimer;
    private float _airMovementMultiplier;
    private float _jumpForce;
    private float _jumpCooldown;
    private float _maxVelocity;
    private float _acceleration;
    private float _decelleration;
    private float _jumpForceMultiplier;
    private const float GRID_CLAMP_THRESHOLD = 0.02f;

    public void ChangeInputSettings()
    {
        if (!IsGrounded())
        {
            return;
        }

        if (_decelleration == _playerStats.GetPlayerMovementDecelleration())
        {
            SetAlternativeStats();
        }
        else
        {
            SetBaseStats();
        }
    }

    private void SetBaseStats()
    {
        _jumpForce = _playerStats.GetJumpForce();
        _jumpCooldown = _playerStats.GetJumpCooldown();
        _maxVelocity = _playerStats.GetMaxVelocity();
        _acceleration = _playerStats.GetPlayerMovementAcceleration();
        _decelleration = _playerStats.GetPlayerMovementDecelleration();
        _jumpForceMultiplier = _playerStats.GetJumpForceMultiplier();
        _airMovementMultiplier = _playerStats.GetAirMovementMultiplier();
    }

    private void SetAlternativeStats()
    {
        // _jumpForce = _playerStats.GetJumpForceAlternative();
        // _jumpCooldown = _playerStats.GetJumpCooldownAlternative();
        // _maxVelocity = _playerStats.GetMaxVelocityAlternative();
        // _acceleration = _playerStats.GetPlayerMovementAccelerationAlternative();
        // _decelleration = _playerStats.GetPlayerMovementDecellerationAlternative();
        // _jumpForceMultiplier = _playerStats.GetJumpForceMultiplierAlternative();
        // _airMovementMultiplier = _playerStats.GetAirMovementMultiplierAlternative();
    }

    // Start is called before the first frame update
    void Awake()
    {
        GameController.SetInputLockState(true);
        _playerStats = gameObject.GetComponent<PlayerStats>();
        SetBaseStats();
        groundCheckDimensions = new Vector3(0.5f, 0.05f, 0.5f);
        Physics.gravity = new Vector3(0, -9.81f, 0);
        _playerRigidBody = GetComponent<Rigidbody>();
        _menu = gameObject.GetComponentInChildren<IngameMenu>();
        levelTarget = GameObject.FindWithTag("Target").gameObject.transform;
        StartCoroutine(SwitchInputLock());
    }

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
        transform.position = new Vector3(transform.position.x, transform.position.y, Constants.PLAYER_Z_VALUE);

        if (_playerRigidBody.velocity.magnitude == 0)
        {
            ClampToGrid();
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
        //}

        if (transform.position != newPosition)
        {
            transform.position = newPosition;
        }
    }

    private bool IsGoalReached()
    {
        return (Vector3.Distance(gameObject.transform.position, levelTarget.position) < 0.5f && IsGrounded());
    }

    public bool IsGrounded()
    {
        return Physics.BoxCast(transform.position, groundCheckDimensions, -transform.up, transform.rotation,
            transform.localScale.y / 2, groundMask);
    }

    // Called by input system
    public void Interact()
    {
        if (IsGoalReached())
        {
            // TODO: SAVE HIGHSCORE AFTER FETCHING TIMER IN LEVELTIMER
            _menu.interactText.SetActive(false);
            _menu.Pause(1);
        }
    }

    // Called by input system
    public void Jump()
    {
        if (_jumpCooldownTimer <= 0 && IsGrounded())
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
        Vector3 dir = new Vector3();
        if (GravityController.IsGravityHorizontal())
        {
            boxCastDimensions = new Vector3(0.47f, 0.01f, 0.47f);
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
            boxCastDimensions = new Vector3(0.01f, 0.47f, 0.47f);
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
            return _playerRigidBody.velocity.magnitude < _maxVelocity && !BoxCast(dir);
        }

        return moveCoefficient != 0 && _playerRigidBody.velocity.magnitude < _maxVelocity;
    }

    private bool BoxCast(Vector3 direction)
    {
        RaycastHit hit;
        Physics.BoxCast(transform.position, boxCastDimensions, direction, out hit, Quaternion.identity,
            transform.localScale.y / 2, groundMask);
        ExtDebug.DrawBoxCastOnHit(transform.position, boxCastDimensions, Quaternion.identity, direction,
            hit.distance, Color.red);

        return hit.collider;
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
        transform.rotation = Quaternion.LookRotation(transform.forward, -GravityController.GetCurrentFacing());
    }
}