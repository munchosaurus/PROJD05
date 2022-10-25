using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    private PlayerMovement _playerController;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float impactMultiplier;
    [SerializeField] private Material[] crosshairMaterials;
    [SerializeField] private LineRenderer _lineRenderer;
    private Vector3 _currentDirection;
    private GameObject crosshair;
    private MeshRenderer crosshairMesh;
    private bool buttonPressed;

    private void Awake()
    {
        _playerController = gameObject.GetComponent<PlayerMovement>();
        crosshair = GameObject.FindGameObjectWithTag("Crosshair");
        crosshairMesh = crosshair.GetComponentInChildren<MeshRenderer>();
        _lineRenderer = GameObject.FindWithTag("LineRenderer").GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        if (crosshair == null)
        {
            return;
        }
        
        _currentDirection = GetMousePositionOnPlane() - transform.position;
        _lineRenderer.SetPosition(0, transform.position);

        Debug.Log(_currentDirection);
        
        if (buttonPressed)
        {
            if (!_lineRenderer.gameObject.activeSelf)
            {
                _lineRenderer.gameObject.SetActive(true);
            }

            if (!crosshair.activeSelf)
            {
                crosshair.SetActive(true);
            }
            SetCrosshair();
        }
        else
        {
            _lineRenderer.gameObject.SetActive(false);
            crosshair.SetActive(false);
        }
    }

    private void SetCrosshair()
    {
        RaycastHit groundHit;
        RaycastHit gravityHit;
        Physics.Raycast(transform.position, _currentDirection, out groundHit, Mathf.Infinity, groundMask);
        Physics.Raycast(transform.position, _currentDirection, out gravityHit, Mathf.Infinity,
            _playerController.gravityChangeLayer);
        Vector3 groundPoint = new Vector3(groundHit.point.x, groundHit.point.y, 0);
        if (gravityHit.collider)
        {
            Vector3 gravityPoint = new Vector3(gravityHit.point.x, gravityHit.point.y, 0);
            if (Vector3.Distance(transform.position, gravityPoint) <=
                Vector3.Distance(transform.position, groundPoint))
            {
                crosshair.transform.position = gravityPoint * impactMultiplier;
                crosshairMesh.material = crosshairMaterials[0];
            }
            else
            {
                crosshair.transform.position = groundPoint * impactMultiplier;
                crosshairMesh.material = crosshairMaterials[1];
            }
        }
        else
        {
            crosshair.transform.position = groundPoint * impactMultiplier;
            crosshairMesh.material = crosshairMaterials[1];
        }
        _lineRenderer.SetPosition(1, crosshair.transform.position);
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
        RaycastHit hit;

        Physics.Raycast(transform.position, _currentDirection, out groundHit, Mathf.Infinity, groundMask);
        if (Physics.Raycast(transform.position, _currentDirection, out hit, Mathf.Infinity,
                _playerController.gravityChangeLayer,
                QueryTriggerInteraction.Collide) && GravityController.GetCurrentFacing() !=
            -hit.normal * GravityController.GetGravity())
        {
            if (Vector3.Distance(transform.position, hit.point) <=
                Vector3.Distance(transform.position, groundHit.point))
            {
                TriggerGravityGunEvent(hit);
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
            crosshair.SetActive(false);
            ShootGravityGun();
        }
    }

    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, Vector3.zero);
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }
}