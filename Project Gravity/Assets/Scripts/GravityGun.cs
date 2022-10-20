using System;
using UnityEngine;

public class GravityGun : MonoBehaviour
{
    private PlayerMovement _playerController;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float ImpactMultiplier;
    [SerializeField] private Material[] crosshairMaterials;
    private Vector3 _currentDirection;
    private GameObject crosshair;
    private MeshRenderer crosshairMesh;

    private void Awake()
    {
        _playerController = gameObject.GetComponent<PlayerMovement>();
        crosshair = GameObject.FindGameObjectWithTag("Crosshair");
        crosshairMesh = crosshair.GetComponentInChildren<MeshRenderer>();
    }

    void FixedUpdate()
    {
        if (crosshair == null)
        {
            return;
        }

        _currentDirection = GetMousePositionOnPlane() - transform.position;

        SetCrosshair();

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
                crosshair.transform.position = gravityPoint * ImpactMultiplier;
                crosshairMesh.material = crosshairMaterials[0];
            }
            else
            {
                crosshair.transform.position = groundPoint * ImpactMultiplier;
                crosshairMesh.material = crosshairMaterials[1];
            }
        }
        else
        {
            crosshair.transform.position = groundPoint * ImpactMultiplier;
            crosshairMesh.material = crosshairMaterials[1];
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

    private Vector3 GetMousePositionOnPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xy = new Plane(Vector3.forward, Vector3.zero);
        xy.Raycast(ray, out float distance);

        return ray.GetPoint(distance);
    }


    // For troubleshooting
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, GetMousePositionOnPlane() - transform.position, Color.green);
    }
}