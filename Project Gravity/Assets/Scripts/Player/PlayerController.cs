using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
    private Vector3 groundCheckDimensions;
    [SerializeField] private Vector3 roofCheckDimensions;
    private AudioSource _audioSource;
    private const float GridClampThreshold = 0.02f;
    private const float PlayerCollisionGridClamp = 0.5f;
    private Guid _playerSucceedsGuid;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.Current.RegisterListener<WinningEvent>(OnPlayerSucceedsLevel, ref _playerSucceedsGuid);
        GameController.PauseGame();
        StartCoroutine(SwitchInputLock());
        _audioSource = GetComponent<AudioSource>();
        velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        velocity += Physics.gravity * Time.fixedDeltaTime;
        MovePlayer();
        ApplyCollisions();

        transform.position += velocity * Time.fixedDeltaTime;
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, 0, eulerRotation.z);

        if (velocity.magnitude == 0)
        {
            ClampToGrid();
        }
    }
    
    private void OnPlayerSucceedsLevel(WinningEvent winningEvent)
    {
        _audioSource.mute = true;
    }

    /*
    * Turns the player input off for as many seconds as there are set in Constants.LEVEL_LOAD_INPUT_PAUSE_TIME
    */
    private IEnumerator SwitchInputLock()
    {
        yield return new WaitForSecondsRealtime(Constants.LEVEL_LOAD_INPUT_PAUSE_TIME);

        if (FindObjectOfType<LevelSettings>().IsTutorialLevel() && GameController.TutorialIsOn)
        {
            FindObjectOfType<IngameMenu>().ToggleActionMap(true);
            GameObject.Find("Tutorial").GetComponent<Tutorial>().BeginTutorial();
        }
        else
        {
            if (GameController.IsPaused())
            {
                GameController.UnpauseGame();
            }
            else
            {
                GameController.PauseGame();
            }
        }
    }

    private void ClampToGrid()
    {
        Vector3 newPosition = transform.position;
        if (Math.Abs(gameObject.transform.position.x - Math.Round(gameObject.transform.position.x)) <
            GridClampThreshold && !GravityController.IsGravityHorizontal())
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
        return Physics.BoxCast(transform.position, groundCheckDimensions, -transform.up, transform.rotation,
            transform.localScale.y / 2, groundMask);
    }

    public bool IsRoofed()
    {
        return Physics.BoxCast(transform.position, roofCheckDimensions, transform.up, transform.rotation,
            transform.localScale.y / 2, groundMask);
    }

    /*
     * Called by input system, adds _jumpForce to the velocity that is opposite to the
     * gravity in case the player is grounded and has passed the jump cool down.
     */
    public void Jump(InputAction.CallbackContext context)
    {
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        if (context.started)
        {
            if (IsGrounded() && !IsRoofed())
            {
                if (GravityController.IsGravityHorizontal())
                {
                    if (Physics.gravity.x > 0 && !groundedLeft)
                    {
                        velocity.x -= GameController.PlayerJumpForce * GameController.GlobalSpeedMultiplier;
                    }

                    if (Physics.gravity.x < 0 && !groundedRight)
                    {
                        velocity.x += GameController.PlayerJumpForce * GameController.GlobalSpeedMultiplier;
                    }
                }
                else
                {
                    if (Physics.gravity.y > 0 && !groundedDown)
                    {
                        velocity.y -= GameController.PlayerJumpForce * GameController.GlobalSpeedMultiplier;
                    }

                    if (Physics.gravity.y < 0 && !groundedUp)
                    {
                        velocity.y += GameController.PlayerJumpForce * GameController.GlobalSpeedMultiplier;
                    }
                }
            }
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
        RaycastHit[] raycastHits;
        if (direction.y != 0 && Math.Abs(velocity.y) >
            Constants.COLLISION_SPEED_THRESHOLD * GameController.GlobalSpeedMultiplier)
        {
            raycastHits = Physics.BoxCastAll(transform.position, verticalCast, direction,
                Quaternion.identity,
                Mathf.Abs(transform.position.y - (transform.position + (velocity * Time.fixedDeltaTime)).y) +
                PlayerCollisionGridClamp, soundCollisionMask, QueryTriggerInteraction.UseGlobal);
            foreach (var collision in raycastHits)
            {
                if (collision.transform.GetComponentInParent<DynamicObjectMovement>())
                {
                    if (collision.transform.GetComponentInParent<DynamicObjectMovement>().velocity.magnitude <
                        Constants.COLLISION_SPEED_THRESHOLD * GameController.GlobalSpeedMultiplier)
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
        else if (direction.x != 0 && Math.Abs(velocity.x) >
                 Constants.COLLISION_SPEED_THRESHOLD * GameController.GlobalSpeedMultiplier)
        {
            raycastHits = Physics.BoxCastAll(transform.position, horizontalCast, direction,
                Quaternion.identity,
                Mathf.Abs(transform.position.x - (transform.position + (velocity * Time.fixedDeltaTime)).x) +
                PlayerCollisionGridClamp, soundCollisionMask, QueryTriggerInteraction.UseGlobal);
            foreach (var collision in raycastHits)
            {
                if (collision.transform.GetComponentInParent<DynamicObjectMovement>())
                {
                    if (collision.transform.GetComponentInParent<DynamicObjectMovement>().velocity.magnitude <
                        Constants.COLLISION_SPEED_THRESHOLD * GameController.GlobalSpeedMultiplier)
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

    /*
     * Dampens movement if needed, will check if the current velocity will place the player within a cube.
     * Also calls upon method handling collision sounds.
     */
    private void ApplyCollisions()
    {
        groundedDown = false;
        groundedUp = false;
        groundedLeft = false;
        groundedRight = false;
        RaycastHit hit;
        Vector3 nextPos = transform.position + (velocity * Time.fixedDeltaTime);
        if (velocity.y < 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.down, out hit, Quaternion.identity,
                    Mathf.Abs(transform.position.y - nextPos.y) + PlayerCollisionGridClamp, groundMask))
            {
                if (Math.Abs(transform.position.y - nextPos.y) < Math.Abs(transform.position.y - hit.point.y))
                {
                    if (!ShouldInheritMovement(hit.collider.gameObject, false))
                    {
                        groundedDown = true;
                    }
                    CheckCollisionInMovement(Vector3.down);
                    transform.position = new Vector3(transform.position.x, hit.point.y + PlayerCollisionGridClamp,
                        transform.position.z);
                    velocity.y = 0;
                }
            }
        }
        else if (velocity.y > 0)
        {
            if (Physics.BoxCast(transform.position, verticalCast, Vector3.up, out hit, Quaternion.identity,
                    Mathf.Abs(transform.position.y - nextPos.y) + PlayerCollisionGridClamp, groundMask))
            {
                if (Math.Abs(transform.position.y - nextPos.y) < Math.Abs(transform.position.y - hit.point.y))
                {
                    if (!ShouldInheritMovement(hit.collider.gameObject, false))
                    {
                        groundedUp = true;
                    }
                    CheckCollisionInMovement(Vector3.up);
                    transform.position = new Vector3(transform.position.x, hit.point.y - PlayerCollisionGridClamp,
                        transform.position.z);
                    velocity.y = 0;
                }
            }
        }

        if (velocity.x < 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.left, out hit, Quaternion.identity,
                    Mathf.Abs(transform.position.x - nextPos.x) + PlayerCollisionGridClamp, groundMask))
            {
                if (Math.Abs(transform.position.x - nextPos.x) < Math.Abs(transform.position.x - hit.point.x))
                {
                    if (!ShouldInheritMovement(hit.collider.gameObject, true))
                    {
                        groundedLeft = true;
                    }
                    CheckCollisionInMovement(Vector3.left);
                    transform.position = new Vector3(hit.point.x + PlayerCollisionGridClamp, transform.position.y,
                        transform.position.z);
                    velocity.x = 0;
                }
            }
        }
        else if (velocity.x > 0)
        {
            if (Physics.BoxCast(transform.position, horizontalCast, Vector3.right, out hit, Quaternion.identity,
                    Mathf.Abs(transform.position.x - nextPos.x) + PlayerCollisionGridClamp, groundMask))
            {
                if (Math.Abs(transform.position.x - nextPos.x) < Math.Abs(transform.position.x - hit.point.x))
                {
                    if (!ShouldInheritMovement(hit.collider.gameObject, true))
                    {
                        groundedRight = true;
                    }
                    CheckCollisionInMovement(Vector3.right);
                    transform.position = new Vector3(hit.point.x - PlayerCollisionGridClamp, transform.position.y,
                        transform.position.z);
                    velocity.x = 0;
                }
            }
        }
    }

    private void MovePlayer()
    {
        if (!IsGrounded())
        {
            if (GravityController.IsGravityHorizontal())
            {
                MoveVertical(_movementKeyInfo.ReadValue<Vector2>().y * GameController.PlayerAirMovementMultiplier);
            }
            else
            {
                MoveHorizontal(_movementKeyInfo.ReadValue<Vector2>().x * GameController.PlayerAirMovementMultiplier);
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
        {
            velocity.x += GameController.GlobalSpeedMultiplier * (direction * GameController.PlayerAcceleration * Time.fixedDeltaTime);
        }
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

        if (ShouldAddMoreMoveForce(direction * GameController.GlobalSpeedMultiplier))
        {
            velocity.y += GameController.GlobalSpeedMultiplier * (direction * GameController.PlayerAcceleration * Time.fixedDeltaTime);
        }
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
            return Math.Abs(magnitude) < GameController.PlayerMaxVelocity * GameController.GlobalSpeedMultiplier && !BoxCast(dir);
        }

        return moveCoefficient != 0 && Math.Abs(magnitude) < GameController.PlayerMaxVelocity * GameController.GlobalSpeedMultiplier;
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