using System;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    [SerializeField] private Material[] crosshairMaterials;
    [SerializeField] private Material[] lineMaterials;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask gravityMask;
    private Vector3 _currentDirection;
    private GameObject crosshair;
    private MeshRenderer crosshairMesh;
    private bool buttonPressed;

    private void Awake()
    {
        //crosshair = GameObject.FindGameObjectWithTag("Crosshair");
        //crosshairMesh = crosshair.GetComponent<MeshRenderer>();
        //crosshairMesh.enabled = false;
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

            // if (crosshairMesh.enabled == false)
            // {
            //     crosshairMesh.enabled = true;
            // }
            SetCrosshair();
        }
        else
        {
            _lineRenderer.gameObject.SetActive(false);
            //crosshairMesh.enabled = false;
        }
    }

    private void SetCrosshair()
    {
        RaycastHit groundHit;
        RaycastHit gravityHit;
        Physics.Raycast(transform.position, _currentDirection, out groundHit, Mathf.Infinity, groundMask);
        Physics.Raycast(transform.position, _currentDirection, out gravityHit, Mathf.Infinity,
            gravityMask);
        Vector3 groundPoint = new Vector3(groundHit.point.x, groundHit.point.y, 1);
        Vector3 linePosition;
        if (gravityHit.collider)
        {
            Vector3 gravityPoint = new Vector3(gravityHit.point.x, gravityHit.point.y, 1);
            if (Vector3.Distance(transform.position, gravityPoint) <=
                Vector3.Distance(transform.position, groundPoint))
            {
                _lineRenderer.material = lineMaterials[1];
                linePosition = gravityPoint * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
                //crosshairMesh.material = crosshairMaterials[0];
            }
            else
            {
                _lineRenderer.material = lineMaterials[0];
                linePosition = groundPoint * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
            }
        }
        else
        {
            _lineRenderer.material = lineMaterials[0];
            linePosition = groundPoint * Constants.PLAYER_AIMING_POINT_POSITIONING_MULTIPLIER;
            //crosshairMesh.material = crosshairMaterials[1];
        }
        _lineRenderer.SetPosition(1, linePosition);
    }

    private void TriggerGravityGunEvent(RaycastHit hit)
    {
        Debug.Log("jajamän");
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

        Physics.Raycast(transform.position, _currentDirection, out groundHit, Mathf.Infinity, groundMask);
        if (Physics.Raycast(transform.position, _currentDirection, out gravityHit, Mathf.Infinity,
                gravityMask,
                QueryTriggerInteraction.Collide) && GravityController.GetCurrentFacing() !=
            -gravityHit.normal)
        {
            if (Vector3.Distance(transform.position, gravityHit.point) <=
                Vector3.Distance(transform.position, groundHit.point))
            {
                TriggerGravityGunEvent(gravityHit);
            }
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
            //crosshairMesh.enabled = false;
            ShootGravityGun();
        }
    }

    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0,0,1));
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }
}