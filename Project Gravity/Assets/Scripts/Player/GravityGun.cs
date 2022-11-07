using System;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    [SerializeField] private Material[] lineMaterials;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask gravityMask;
    [SerializeField] private LayerMask magnetMask;
    private LineRenderer _lineRenderer;
    private GameObject aimDirector;
    private Vector3 _currentDirection;
    private GameObject crosshair;
    private MeshRenderer crosshairMesh;
    private bool buttonPressed;

    private void Awake()
    {
        aimDirector = GameObject.FindGameObjectWithTag("AimingDirector");
        _lineRenderer = GameObject.FindWithTag("LineRenderer").GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        if (GameController.GetPlayerInputIsLocked())
        {
            return;
        }

        _currentDirection = GetMousePositionOnPlane() - transform.position;
        _lineRenderer.SetPosition(0, transform.position);

        if (buttonPressed)
        {
            if (!_lineRenderer.gameObject.activeSelf)
            {
                _lineRenderer.gameObject.SetActive(true);
            }

            SetCrosshair();
        }
        else
        {
            _lineRenderer.gameObject.SetActive(false);
        }
    }

    private void SetCrosshair()
    {
        Physics.Raycast(transform.position, _currentDirection, out var groundHit, Mathf.Infinity, groundMask);
        Physics.Raycast(transform.position, _currentDirection, out var gravityHit, Mathf.Infinity,
            gravityMask);
        Physics.Raycast(transform.position, _currentDirection, out var magnetHit, Mathf.Infinity,
            magnetMask);
        Vector3 linePosition;
        if (gravityHit.collider)
        {
            if (magnetHit.collider)
            {
                if ((float) Math.Round(Vector3.Distance(transform.position, gravityHit.point), 2) >
                    (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
                {
                    if ((float) Math.Round(Vector3.Distance(transform.position, magnetHit.point), 2) >
                        (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
                    {
                        linePosition = SetGroundAim(groundHit);
                    }
                    else
                    {
                        if (magnetHit.collider.GetComponentInParent<DynamicObjectMovement>() != null)
                        {
                            if (magnetHit.collider.GetComponentInParent<DynamicObjectMovement>().lockedToMagnet)
                            {
                                linePosition = SetMagnetAim(magnetHit);
                            }
                            else
                            {
                                linePosition = SetGroundAim(groundHit);
                            }
                        }
                        else
                        {
                            linePosition = SetMagnetAim(magnetHit);
                        }
                    }
                }
                else
                {
                    linePosition = SetGravityAim(gravityHit);
                }
            }
            else
            {
                // Where no magnet exists 
                if ((float) Math.Round(Vector3.Distance(transform.position, gravityHit.point), 2) >
                    (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
                {
                    linePosition = SetGroundAim(groundHit);
                }
                else
                {
                    linePosition = SetGravityAim(gravityHit);
                }
            }
        }
        else if (magnetHit.collider)
        {
            // Where no gravity exists but magnet does
            if ((float) Math.Round(Vector3.Distance(transform.position, magnetHit.point), 2) >
                (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
            {
                linePosition = SetGroundAim(groundHit);
            }
            else
            {
                if (magnetHit.collider.GetComponentInParent<DynamicObjectMovement>() != null)
                {
                    if (magnetHit.collider.GetComponentInParent<DynamicObjectMovement>().lockedToMagnet)
                    {
                        linePosition = SetMagnetAim(magnetHit);
                    }
                    else
                    {
                        linePosition = SetGroundAim(groundHit);
                    }
                }
                else
                {
                    linePosition = SetMagnetAim(magnetHit);
                }
            }
        }
        else
        {
            linePosition = SetGroundAim(groundHit);
        }

        _lineRenderer.SetPosition(1, linePosition);
    }

    private Vector3 SetGroundAim(RaycastHit hit)
    {
        DisableAimDirector();
        _lineRenderer.material = lineMaterials[0];
        return hit.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
    }

    private Vector3 SetGravityAim(RaycastHit hit)
    {
        EnableAimDirector(hit);
        _lineRenderer.material = lineMaterials[1];
        return hit.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
    }

    private Vector3 SetMagnetAim(RaycastHit hit)
    {
        DisableAimDirector();
        _lineRenderer.material = lineMaterials[2];
        return hit.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
    }

    private void EnableAimDirector(RaycastHit hit)
    {
        if (!aimDirector.GetComponent<SpriteRenderer>().enabled)
        {
            aimDirector.GetComponent<SpriteRenderer>().enabled = true;
        }

        aimDirector.transform.position = new Vector3(hit.point.x,
            hit.point.y - 1, 2);

        if (hit.normal.x != 0)
        {
            if (hit.point.x > transform.position.x)
            {
                aimDirector.transform.position = new Vector3(hit.point.x - 0.5f,
                    hit.point.y, 2);
            }
            else
            {
                aimDirector.transform.position = new Vector3(hit.point.x + 0.5f,
                    hit.point.y, 2);
            }
        }
        else if (hit.normal.y != 0)
        {
            if (hit.point.y > transform.position.y)
            {
                aimDirector.transform.position = new Vector3(hit.point.x,
                    hit.point.y - 0.5f, 2);
            }
            else
            {
                aimDirector.transform.position = new Vector3(hit.point.x,
                    hit.point.y + 0.5f, 2);
            }
        }

        aimDirector.transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
    }

    private void DisableAimDirector()
    {
        if (aimDirector.GetComponent<SpriteRenderer>().enabled)
        {
            aimDirector.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void TriggerGravityGunEvent(RaycastHit hit)
    {
        Event gravityGunEvent = new GravityGunEvent()
        {
            TargetGameObject = hit.transform.gameObject,
            SourceGameObject = gameObject,
            hitNormal = hit.normal
        };
        EventSystem.Current.FireEvent(gravityGunEvent);
    }

    public void ShootGravityGun()
    {
        Physics.Raycast(transform.position, _currentDirection, out var groundHit, Mathf.Infinity, groundMask);
        Physics.Raycast(transform.position, _currentDirection, out var gravityHit, Mathf.Infinity,
            gravityMask);
        Physics.Raycast(transform.position, _currentDirection, out var magnetHit, Mathf.Infinity,
            magnetMask);
        if (gravityHit.collider)
        {
            if (magnetHit.collider)
            {
                if ((float) Math.Round(Vector3.Distance(transform.position, gravityHit.point), 2) >
                    (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
                {
                    if ((float) Math.Round(Vector3.Distance(transform.position, magnetHit.point), 2) >
                        (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
                    {
                        // Condition where ground is closest
                        TriggerGravityGunEvent(groundHit);
                    }
                    else
                    {
                        // Condition where magnet is closer than ground
                        TriggerGravityGunEvent(magnetHit);
                    }
                }
                else
                {
                    // Condition where gravity is closer than ground (will also be closer than magnet since magnet and ground are in same pos)
                    TriggerGravityGunEvent(gravityHit);
                }
            }
            else
            {
                // Where no magnet exists 
                if ((float) Math.Round(Vector3.Distance(transform.position, gravityHit.point), 2) >
                    (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
                {
                    // Condition where ground is closer than gravity and magnet
                    TriggerGravityGunEvent(groundHit);
                }
                else
                {
                    // Condition where gravity is closer than ground and magnet
                    TriggerGravityGunEvent(gravityHit);
                }
            }
        }
        else if (magnetHit.collider)
        {
            // Where no gravity exists but magnet does
            if ((float) Math.Round(Vector3.Distance(transform.position, magnetHit.point), 2) >
                (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
            {
                // Condition where ground is closer than both magnet and gravity
                TriggerGravityGunEvent(groundHit);
            }
            else
            {
                // Condition where magnet is closer than ground
                TriggerGravityGunEvent(magnetHit);
            }
        }
        else
        {
            // Where only ground is hit
            TriggerGravityGunEvent(groundHit);
        }

        DisableAimDirector();
    }

    public void Aim(InputAction.CallbackContext val)
    {
        if (val.performed)
        {
            buttonPressed = true;
        }
        else if (!val.performed)
        {
            buttonPressed = false;
        }

        if (val.canceled)
        {
            _lineRenderer.gameObject.SetActive(false);
            ShootGravityGun();
        }
    }

    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 1));
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }
}