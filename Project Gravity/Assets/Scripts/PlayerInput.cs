using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Vector3 velocity;
    [SerializeField] private Vector3 horizontalCast, verticalCast;
    [SerializeField] bool groundedRight;
    [SerializeField] bool groundedLeft;
    [SerializeField] bool groundedUp;
    [SerializeField] bool groundedDown;
    [SerializeField] private float friction;
    [SerializeField] private LayerMask groundMask;

    private readonly float GRID_OFFSET = 0;
    private readonly float OBJECT_Z = 1;
    private Vector3 _boxCastDimensions;

    // from playermovement
    [SerializeField] private Transform levelTarget;

    private InputAction.CallbackContext _movementKeyInfo;
    private IngameMenu _menu;
    private PlayerStats _playerStats;
    private Vector3 _groundCheckDimensions;
    private float _jumpCooldownTimer;
    private float _airMovementMultiplier;
    private float _jumpForce;
    private float _jumpCooldown;
    private float _maxVelocity;
    private float _acceleration;
    private float _decelleration;
    private float _jumpForceMultiplier;
    private const float GRID_CLAMP_THRESHOLD = 0.02f;
    private bool hasJumped;
    private const float DISTANCE_TO_FINISHED_THRESHOLD = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        GameController.SetInputLockState(true);
        _playerStats = gameObject.GetComponent<PlayerStats>();
        SetBaseStats();
        _groundCheckDimensions = new Vector3(0.5f, 0.05f, 0.5f);
        Physics.gravity = new Vector3(0, -Constants.GRAVITY, 0);
        _menu = gameObject.GetComponentInChildren<IngameMenu>();
        levelTarget = GameObject.FindWithTag("Target").gameObject.transform;
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

        CheckForCollisions();
        ApplyFriction();
        ApplyCollisions();


        transform.position += velocity * Time.fixedDeltaTime;

        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, 0, eulerRotation.z);
        transform.position = new Vector3(transform.position.x, transform.position.y, Constants.PLAYER_Z_VALUE);
        Debug.Log(hasJumped);
        if (velocity.magnitude == 0)
        {
            ClampToGrid();
        }
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
        _jumpForce = _playerStats.GetJumpForceAlternative();
        _jumpCooldown = _playerStats.GetJumpCooldownAlternative();
        _maxVelocity = _playerStats.GetMaxVelocityAlternative();
        _acceleration = _playerStats.GetPlayerMovementAccelerationAlternative();
        _decelleration = _playerStats.GetPlayerMovementDecellerationAlternative();
        _jumpForceMultiplier = _playerStats.GetJumpForceMultiplierAlternative();
        _airMovementMultiplier = _playerStats.GetAirMovementMultiplierAlternative();
    }

    private void ClampToGrid()
    {
        Vector3 newPosition = transform.position;
        if (Math.Abs(gameObject.transform.position.x - Math.Round(gameObject.transform.position.x)) <
            GRID_CLAMP_THRESHOLD && !GravityController.IsGravityHorizontal())
        {
            newPosition.x = Mathf.Round(transform.position.x);
        }

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
            if (GravityController.IsGravityHorizontal())
            {
                if (Physics.gravity.x > 0)
                {
                    velocity.x += _jumpForce;
                }
                else
                {
                    velocity.x -= _jumpForce;
                }
            }
            else
            {
                if (Physics.gravity.y > 0)
                {
                    velocity.y -= _jumpForce;
                }
                else
                {
                    velocity.y += _jumpForce;
                }
            }

            hasJumped = true;
            _jumpCooldownTimer = _jumpCooldown;
        }
    }

    // Called by input system
    public void SetMovementInput(InputAction.CallbackContext movement)
    {
        _movementKeyInfo = movement;
    }

    private bool IsGoalReached()
    {
        return (Vector3.Distance(gameObject.transform.position, levelTarget.position) <
            DISTANCE_TO_FINISHED_THRESHOLD && IsGrounded());
    }

    private bool ShouldInheritMovement(GameObject otherObject, bool isHorizontal)
    {
        if (otherObject.GetComponent<DynamicObjectMovement>() != null)
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
                    transform.localScale.y / 2, groundMask))
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
            }
        }

        if (velocity.x > 0)
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
            }
        }
        else if (velocity.x < 0)
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

    private void MovePlayer()
    {
        if (!IsGrounded())
        {
            if (!hasJumped)
                return;
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
            velocity.x += direction * _acceleration;
    }

    private void MoveVertical(float direction)
    {
        if (ShouldAddMoreMoveForce(direction))
            velocity.y += direction * _acceleration;
    }

    private bool ShouldAddMoreMoveForce(float moveCoefficient)
    {
        Vector3 dir = new Vector3();
        if (GravityController.IsGravityHorizontal())
        {
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
            return velocity.magnitude < _maxVelocity && !BoxCast(dir);
        }

        return moveCoefficient != 0 && velocity.magnitude < _maxVelocity;
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

    private void ClampMoveSpeed()
    {
        if (velocity.magnitude > _maxVelocity)
        {
            velocity = velocity.normalized * _maxVelocity;
        }
    }

    public void RotateToPlane()
    {
        transform.rotation = Quaternion.LookRotation(transform.forward, -GravityController.GetCurrentFacing());
    }
}