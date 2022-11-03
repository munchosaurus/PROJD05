using System;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    [SerializeField] private Material[] lineMaterials;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask gravityMask;
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
        RaycastHit groundHit;
        RaycastHit gravityHit;
        Physics.Raycast(transform.position, _currentDirection, out groundHit, Mathf.Infinity, groundMask);
        Physics.Raycast(transform.position, _currentDirection, out gravityHit, Mathf.Infinity,
            gravityMask);
        Vector3 linePosition;
        if (gravityHit.collider)
        {
            if ((float) Math.Round(Vector3.Distance(transform.position, gravityHit.point), 2) >
                (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
            {
                DisableAimDirector();
                _lineRenderer.material = lineMaterials[0];
                linePosition = groundHit.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
            }
            else
            {
                _lineRenderer.material = lineMaterials[1];
                EnableAimDirector(gravityHit);
                linePosition = gravityHit.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
            }
        }
        else
        {
            DisableAimDirector();
            _lineRenderer.material = lineMaterials[0];
            linePosition = groundHit.point * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
        }

        _lineRenderer.SetPosition(1, linePosition);
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
        RaycastHit groundHit;
        RaycastHit gravityHit;
        DisableAimDirector();
        Physics.Raycast(transform.position, _currentDirection, out groundHit, Mathf.Infinity, groundMask);
        if (Physics.Raycast(transform.position, _currentDirection, out gravityHit, Mathf.Infinity,
                gravityMask,
                QueryTriggerInteraction.Collide) && GravityController.GetCurrentFacing() !=
            -gravityHit.normal)
        {
            if ((float) Math.Round(Vector3.Distance(transform.position, gravityHit.point), 2) >
                (float) Math.Round(Vector3.Distance(transform.position, groundHit.point), 2))
            {
                return;
            }

            TriggerGravityGunEvent(gravityHit);
        }
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